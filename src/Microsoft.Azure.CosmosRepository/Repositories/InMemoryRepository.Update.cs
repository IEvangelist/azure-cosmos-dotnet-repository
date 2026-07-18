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
        (string Key, TItem Item)? stored = FindStoredItem(value.Id, value.PartitionKey);

        if (value is IItemWithEtag valueWithEtag &&
            !string.IsNullOrWhiteSpace(valueWithEtag.Etag) &&
            stored is { } storedValue &&
            storedValue.Item is IItemWithEtag existingItemWithEtag &&
            !ignoreEtag &&
            valueWithEtag.Etag != existingItemWithEtag.Etag)
        {
            MismatchedEtags();
        }

        string storageKey = stored?.Key ?? InMemoryStorage.GetKey(value.Id, value.PartitionKey);
        items[storageKey] = SerializeItem(value, Guid.NewGuid().ToString(), CurrentTs);
        TItem item = DeserializeItem(items[storageKey]);

        if (raiseChanges)
        {
            PublishChanges(new ChangeFeedItemArgs<TItem>(item));
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

        PublishChanges(new ChangeFeedItemArgs<TItem>(results));

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

        (string Key, TItem Item)? stored = FindStoredItem(id, partitionKeyValue);
        TItem? item = stored is { } storedValue ? storedValue.Item : default;

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
            IReadOnlyList<PropertyInfo> propertyInfos = internalPatchOperation.PropertyInfos;
            object? currentObject = item;

            if (propertyInfos.Count is 0)
            {
                continue;
            }

            for (var i = 0; i < propertyInfos.Count; i++)
            {
                if (i == propertyInfos.Count - 1)
                {
                    propertyInfos[i].SetValue(currentObject, internalPatchOperation.NewValue);
                    break;
                }

                currentObject = propertyInfos[i].GetValue(currentObject);
            }
        }

        ConcurrentDictionary<string, string> items = InMemoryStorage.GetDictionary<TItem>();
        string storageKey = stored!.Value.Key;
        items[storageKey] = SerializeItem(item!, Guid.NewGuid().ToString(), CurrentTs);
        PublishChanges(new ChangeFeedItemArgs<TItem>(DeserializeItem(items[storageKey])));
    }

    private void MismatchedEtags() =>
        throw new CosmosException(string.Empty, HttpStatusCode.PreconditionFailed, 0, string.Empty, 0);
}
