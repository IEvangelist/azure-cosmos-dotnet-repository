namespace Microsoft.Azure.CosmosRepository;

/// <summary>
/// In-memory batch builder that validates every queued operation against a
/// simulated view of storage before committing any of them.
/// </summary>
/// <remarks>
/// A failing operation therefore leaves storage untouched, mirroring the
/// rollback behavior of a Cosmos transactional batch for single-threaded
/// tests. The commit phase is still sequential and provides no isolation
/// from concurrent access.
/// </remarks>
internal sealed class InMemoryBatchBuilder(
    string partitionKey,
    IServiceProvider serviceProvider) : IBatchBuilder
{
    private readonly string _partitionKey = partitionKey;
    private readonly List<BatchOperation> _operations = [];
    private readonly HashSet<Type> _seenTypes = [];

    public IBatchBuilder CreateItem<TItem>(TItem item) where TItem : IItem
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        ValidatePartitionKey(item);
        return Add<TItem>(new(
            async (entries, createdIds, cancellationToken) =>
            {
                SimulatedEntry entry = await GetEntryAsync<TItem>(entries, item.Id, cancellationToken).ConfigureAwait(false);

                if (entry.Exists)
                {
                    throw Conflict();
                }

                if (!createdIds.Add(item.Id))
                {
                    throw Conflict();
                }

                entry.Exists = true;
                entry.Etag = null;
            },
            cancellationToken => CreateAsync(item, cancellationToken),
            () => BatchRepository<TItem>()));
    }

    public IBatchBuilder ReplaceItem<TItem>(TItem item) where TItem : IItem
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        ValidatePartitionKey(item);
        return Add<TItem>(new(
            async (entries, _, cancellationToken) =>
            {
                SimulatedEntry entry = await GetEntryAsync<TItem>(entries, item.Id, cancellationToken).ConfigureAwait(false);

                // Cosmos transactional batches fail a replace with 404 when
                // the item does not exist; an upsert would silently create it.
                if (!entry.Exists)
                {
                    throw NotFound();
                }

                ValidateEtag(item, entry);
                entry.Etag = null;
            },
            cancellationToken => UpsertAsync(item, cancellationToken),
            () => BatchRepository<TItem>()));
    }

    public IBatchBuilder UpsertItem<TItem>(TItem item) where TItem : IItem
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        ValidatePartitionKey(item);
        return Add<TItem>(new(
            async (entries, _, cancellationToken) =>
            {
                SimulatedEntry entry = await GetEntryAsync<TItem>(entries, item.Id, cancellationToken).ConfigureAwait(false);

                ValidateEtag(item, entry);
                entry.Exists = true;
                entry.Etag = null;
            },
            cancellationToken => UpsertAsync(item, cancellationToken),
            () => BatchRepository<TItem>()));
    }

    public IBatchBuilder DeleteItem<TItem>(TItem item) where TItem : IItem
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        ValidatePartitionKey(item);

        return DeleteItem<TItem>(item.Id);
    }

    public IBatchBuilder DeleteItem<TItem>(string id) where TItem : IItem
    {
        if (id is null)
        {
            throw new ArgumentNullException(nameof(id));
        }

        return Add<TItem>(new(
            async (entries, _, cancellationToken) =>
            {
                SimulatedEntry entry = await GetEntryAsync<TItem>(entries, id, cancellationToken).ConfigureAwait(false);

                if (!entry.Exists)
                {
                    throw NotFound();
                }

                entry.Exists = false;
            },
            cancellationToken => DeleteAsync<TItem>(id, cancellationToken),
            () => BatchRepository<TItem>()));
    }

    public async ValueTask ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (_operations.Count == 0)
        {
            throw new ArgumentException(
                "Unable to perform batch operation with no items",
                nameof(_operations));
        }

        cancellationToken.ThrowIfCancellationRequested();

        // Mirrors the same-container validation the Cosmos builder performs
        // through ICosmosContainerService.GetContainerAsync(IReadOnlyList<Type>).
        RepositoryOptions options = serviceProvider.GetRequiredService<IOptions<RepositoryOptions>>().Value;
        ICosmosContainerNameProvider containerNameProvider =
            serviceProvider.GetRequiredService<ICosmosContainerNameProvider>();

        BatchContainerValidation.EnsureSameContainer(
            _seenTypes,
            itemType => options.ContainerPerItemType
                ? containerNameProvider.GetContainerName(itemType)
                : options.ContainerId);

        List<IInMemoryBatchRepository> repositories = _operations
            .Select(operation => operation.Repository())
            .Distinct()
            .ToList();

        // Validate every operation against a simulated view of storage
        // (seeded from current state, updated per operation so batches may
        // reference their own earlier writes) before committing anything.
        Dictionary<(Type ItemType, string Id), SimulatedEntry> entries = [];
        HashSet<string> createdIds = [];

        try
        {
            foreach (BatchOperation operation in _operations)
            {
                await operation.ValidateAsync(entries, createdIds, cancellationToken).ConfigureAwait(false);
            }

            Dictionary<Type, Dictionary<string, string>> snapshot = InMemoryStorage.Snapshot(_seenTypes);

            foreach (IInMemoryBatchRepository repository in repositories)
            {
                repository.BeginBatch();
            }

            try
            {
                foreach (BatchOperation operation in _operations)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await operation.CommitAsync(cancellationToken).ConfigureAwait(false);
                }
            }
            catch
            {
                InMemoryStorage.Restore(snapshot);

                foreach (IInMemoryBatchRepository repository in repositories)
                {
                    repository.RollbackBatch();
                }

                throw;
            }

            // Drain every repository before publishing so a change feed
            // processor that throws cannot leave a later repository buffering.
            List<Action> notifications = repositories
                .SelectMany(repository => repository.CommitBatch())
                .ToList();

            foreach (Action notification in notifications)
            {
                notification();
            }
        }
        catch (CosmosException exception)
        {
            throw new BatchOperationException(exception.StatusCode, exception);
        }
    }

    private void ValidatePartitionKey(IItem item)
    {
        if (!string.Equals(item.PartitionKey, _partitionKey, StringComparison.Ordinal))
        {
            throw new ArgumentException(
                $"The item partition key '{item.PartitionKey}' does not match the batch partition key '{_partitionKey}'.",
                nameof(item));
        }
    }

    private IBatchBuilder Add<TItem>(BatchOperation operation) where TItem : IItem
    {
        EnsureCapacity();
        _seenTypes.Add(typeof(TItem));
        _operations.Add(operation);
        return this;
    }

    private void EnsureCapacity()
    {
        if (_operations.Count >= BatchConstants.MaxBatchSize)
        {
            throw new InvalidOperationException(
                $"A transactional batch cannot contain more than {BatchConstants.MaxBatchSize} operations.");
        }
    }

    private async ValueTask<SimulatedEntry> GetEntryAsync<TItem>(
        Dictionary<(Type ItemType, string Id), SimulatedEntry> entries,
        string id,
        CancellationToken cancellationToken) where TItem : IItem
    {
        (Type, string) key = (typeof(TItem), id);

        if (entries.TryGetValue(key, out SimulatedEntry? entry))
        {
            return entry;
        }

        IRepository<TItem> repository = Repository<TItem>();
        TItem? stored;

        if (repository is InMemoryRepository<TItem> inMemoryRepository)
        {
            (string Key, TItem Item)? storedItem = inMemoryRepository.FindStoredItem(id, _partitionKey);
            stored = storedItem is { } value ? value.Item : default;
        }
        else
        {
            stored = await repository.TryGetAsync(id, _partitionKey, cancellationToken).ConfigureAwait(false);
        }

        entry = new SimulatedEntry
        {
            Exists = stored is not null,
            Etag = (stored as IItemWithEtag)?.Etag
        };

        entries[key] = entry;

        return entry;
    }

    // Mirrors the etag rule of InMemoryRepository.UpdateAsync: a non-empty
    // etag on an existing item must match the stored etag. A simulated write
    // invalidates the etag, so any etag supplied to a later operation is stale.
    private static void ValidateEtag(IItem item, SimulatedEntry entry)
    {
        if (item is not IItemWithEtag itemWithEtag ||
            string.IsNullOrWhiteSpace(itemWithEtag.Etag) ||
            !entry.Exists)
        {
            return;
        }

        if (itemWithEtag.Etag != entry.Etag)
        {
            throw PreconditionFailed();
        }
    }

    private async ValueTask CreateAsync<TItem>(TItem item, CancellationToken cancellationToken) where TItem : IItem =>
        await Repository<TItem>().CreateAsync(item, cancellationToken).ConfigureAwait(false);

    private async ValueTask UpsertAsync<TItem>(TItem item, CancellationToken cancellationToken) where TItem : IItem =>
        await Repository<TItem>().UpdateAsync(item, cancellationToken: cancellationToken).ConfigureAwait(false);

    private async ValueTask DeleteAsync<TItem>(string id, CancellationToken cancellationToken) where TItem : IItem =>
        await Repository<TItem>().DeleteAsync(id, _partitionKey, cancellationToken).ConfigureAwait(false);

    // Resolving through DI targets the same repository instances the
    // InMemoryChangeFeed subscribes to, so batch writes raise change
    // notifications like every other repository operation.
    private IRepository<TItem> Repository<TItem>() where TItem : IItem =>
        serviceProvider.GetRequiredService<IRepository<TItem>>();

    private IInMemoryBatchRepository BatchRepository<TItem>() where TItem : IItem =>
        Repository<TItem>() as IInMemoryBatchRepository ??
        throw new InvalidOperationException(
            $"The registered repository for {typeof(TItem).Name} must be an {typeof(InMemoryRepository<TItem>).Name} to execute an in-memory transactional batch.");

    private static CosmosException NotFound() =>
        new(string.Empty, HttpStatusCode.NotFound, 0, string.Empty, 0);

    private static CosmosException Conflict() =>
        new(string.Empty, HttpStatusCode.Conflict, 0, string.Empty, 0);

    private static CosmosException PreconditionFailed() =>
        new(string.Empty, HttpStatusCode.PreconditionFailed, 0, string.Empty, 0);

    private sealed class BatchOperation(
        Func<Dictionary<(Type ItemType, string Id), SimulatedEntry>, HashSet<string>, CancellationToken, ValueTask> validateAsync,
        Func<CancellationToken, ValueTask> commitAsync,
        Func<IInMemoryBatchRepository> repository)
    {
        public Func<Dictionary<(Type ItemType, string Id), SimulatedEntry>, HashSet<string>, CancellationToken, ValueTask> ValidateAsync { get; } = validateAsync;

        public Func<CancellationToken, ValueTask> CommitAsync { get; } = commitAsync;

        public Func<IInMemoryBatchRepository> Repository { get; } = repository;
    }

    private sealed class SimulatedEntry
    {
        public bool Exists { get; set; }

        public string? Etag { get; set; }
    }
}
