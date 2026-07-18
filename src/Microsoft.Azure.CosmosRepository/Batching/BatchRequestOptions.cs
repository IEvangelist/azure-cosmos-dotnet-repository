namespace Microsoft.Azure.CosmosRepository;

internal static class BatchRequestOptions
{
    internal static TransactionalBatchItemRequestOptions Create(IItem item) =>
        Create(item is IItemWithEtag itemWithEtag ? itemWithEtag.Etag : null);

    internal static TransactionalBatchItemRequestOptions Create(string? etag) =>
        new()
        {
            IfMatchEtag = string.IsNullOrWhiteSpace(etag) ? null : etag
        };
}
