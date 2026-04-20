namespace Microsoft.Azure.CosmosRepository;

/// <summary>
/// Constants that apply to transactional batch operations.
/// </summary>
public static class BatchConstants
{
    /// <summary>
    /// The maximum number of operations allowed in a single transactional batch.
    /// </summary>
    public const int MaxBatchSize = 100;
}
