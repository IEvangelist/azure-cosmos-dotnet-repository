namespace Microsoft.Azure.CosmosRepository.Dtos
{
    /// <inheritdoc />
    public class PagedResultRequest : LimitedResultRequest, IPagedResultRequest
    {
        /// <inheritdoc />
        public int SkipCount { get; set; }
    }
}
