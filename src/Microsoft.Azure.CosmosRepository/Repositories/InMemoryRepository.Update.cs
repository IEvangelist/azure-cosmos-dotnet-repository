// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.


// ReSharper disable once CheckNamespace
namespace Microsoft.Azure.CosmosRepository;

internal partial class InMemoryRepository<TItem>
{
    private async ValueTask<TItem> UpdateAsync(
        TItem value,
        bool raiseChanges,
        bool ignoreEtag = false)
    {
#if NET7_0_OR_GREATER
        await ValueTask.CompletedTask;
#else
        await Task.CompletedTask;
#endif

        ConcurrentDictionary<string, string> items = InMemoryStorage.GetDictionary<TItem>();

        if (value is IItemWithEtag valueWithEtag &&
            !string.IsNullOrWhiteSpace(valueWithEtag.Etag) &&
            items.ContainsKey(value.Id) &&
            DeserializeItem(items[value.Id]) is IItemWithEtag existingItemWithEtag &&
            !ignoreEtag
            && valueWithEtag.Etag != existingItemWithEtag.Etag)
        {
            MismatchedEtags();
        }

        items[value.Id] = SerializeItem(value, Guid.NewGuid().ToString(), CurrentTs);

        TItem item = DeserializeItem(items[value.Id]);

        if (raiseChanges)
        {
            Changes?.Invoke(new ChangeFeedItemArgs<TItem>(item));
        }

        return item;
    }

    /// <inheritdoc/>
    public ValueTask<TItem> UpdateAsync(TItem value,
        bool ignoreEtag = false,
        CancellationToken cancellationToken = default) =>
        UpdateAsync(value, true, ignoreEtag);

    /// <inheritdoc/>
    public async ValueTask<IEnumerable<TItem>> UpdateAsync(IEnumerable<TItem> values,
        bool ignoreEtag = false,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<TItem> enumerable = values.ToList();

        List<TItem> results = [];

        foreach (TItem value in enumerable)
        {
            results.Add(await UpdateAsync(value, false, ignoreEtag));
        }

        Changes?.Invoke(new ChangeFeedItemArgs<TItem>(results));

        return results;
    }

    /// <inheritdoc/>
    public async ValueTask UpdateAsync(string id,
        Action<IPatchOperationBuilder<TItem>> builder,
        string? partitionKeyValue = null,
        string? etag = default,
        CancellationToken cancellationToken = default)
    {
#if NET7_0_OR_GREATER
        await ValueTask.CompletedTask;
#else
        await Task.CompletedTask;
#endif

        partitionKeyValue ??= id;

        TItem? item = InMemoryStorage
            .GetValues<TItem>()
            .Select(DeserializeItem)
            .FirstOrDefault(x => x.Id == id && x.PartitionKey == partitionKeyValue);

        switch (item)
        {
            case null:
                NotFound();
                break;
            case IItemWithEtag itemWithEtag when
                etag != default &&
                !string.IsNullOrWhiteSpace(etag) &&
                itemWithEtag.Etag != etag:
                MismatchedEtags();
                break;
        }

        PatchOperationBuilder<TItem> patchOperationBuilder = new();

        builder(patchOperationBuilder);

        foreach (InternalPatchOperation internalPatchOperation in
                 patchOperationBuilder._rawPatchOperations.Where(ipo => ipo.Type is PatchOperationType.Replace))
        {
            PropertyInfo property = item!.GetType().GetProperty(internalPatchOperation.PropertyInfo.Name)!;
            property.SetValue(item, internalPatchOperation.NewValue);
        }

        ConcurrentDictionary<string, string> items = InMemoryStorage.GetDictionary<TItem>();

        items[id] = SerializeItem(item!, Guid.NewGuid().ToString(), CurrentTs);

        Changes?.Invoke(new ChangeFeedItemArgs<TItem>(DeserializeItem(items[id])));
    }

    private void MismatchedEtags() =>
        throw new CosmosException(string.Empty, HttpStatusCode.PreconditionFailed, 0, string.Empty, 0);
}