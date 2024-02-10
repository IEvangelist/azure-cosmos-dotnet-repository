// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Aspire.Microsoft.Azure.CosmosRepository.Items;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;


namespace Aspire.Microsoft.Azure.CosmosRepository.Tests.Mocks;

public class TestContainer<TItem> : Container where TItem : IItem
{
    private readonly Func<(string id, PartitionKey partitionKey, ItemRequestOptions? options), Task<ItemResponse<TItem>>> _onReadItemAsync;

    private TestContainer(
        Func<(string id, PartitionKey partitionKey, ItemRequestOptions? options), Task<ItemResponse<TItem>>> onReadItemAsync)
    {
        _onReadItemAsync = onReadItemAsync;
    }


    #region Static Test Builders

    public static TestContainer<TItem> New(
        Func<(string id, PartitionKey partitionKey, ItemRequestOptions? options), Task<ItemResponse<TItem>>> onReadItemAsync) =>
        new(onReadItemAsync);

    #endregion

    #region Container Implementation

    public override Task<ContainerResponse> ReadContainerAsync(
        ContainerRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<ResponseMessage> ReadContainerStreamAsync(
        ContainerRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<ContainerResponse> ReplaceContainerAsync(
        ContainerProperties containerProperties,
        ContainerRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<ResponseMessage> ReplaceContainerStreamAsync(
        ContainerProperties containerProperties,
        ContainerRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<ContainerResponse> DeleteContainerAsync(
        ContainerRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<ResponseMessage> DeleteContainerStreamAsync(
        ContainerRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<int?> ReadThroughputAsync(
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<ThroughputResponse> ReadThroughputAsync(
        RequestOptions requestOptions,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<ThroughputResponse> ReplaceThroughputAsync(
        int throughput,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<ThroughputResponse> ReplaceThroughputAsync(
        ThroughputProperties throughputProperties,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<ResponseMessage> CreateItemStreamAsync(
        Stream streamPayload,
        PartitionKey partitionKey,
        ItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<ItemResponse<T>> CreateItemAsync<T>(
        T item,
        PartitionKey? partitionKey = null,
        ItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<ResponseMessage> ReadItemStreamAsync(
        string id,
        PartitionKey partitionKey,
        ItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override async Task<ItemResponse<T>> ReadItemAsync<T>(
        string id,
        PartitionKey partitionKey,
        ItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        return await _onReadItemAsync.Invoke(
            new(
                id,
                partitionKey,
                requestOptions)) as ItemResponse<T>;
    }

    public override Task<ResponseMessage> UpsertItemStreamAsync(
        Stream streamPayload,
        PartitionKey partitionKey,
        ItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<ItemResponse<T>> UpsertItemAsync<T>(
        T item,
        PartitionKey? partitionKey = null,
        ItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<ResponseMessage> ReplaceItemStreamAsync(
        Stream streamPayload,
        string id,
        PartitionKey partitionKey,
        ItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<ItemResponse<T>> ReplaceItemAsync<T>(
        T item,
        string id,
        PartitionKey? partitionKey = null,
        ItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<ResponseMessage> ReadManyItemsStreamAsync(
        IReadOnlyList<(string id, PartitionKey partitionKey)> items,
        ReadManyRequestOptions? readManyRequestOptions = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<FeedResponse<T>> ReadManyItemsAsync<T>(
        IReadOnlyList<(string id, PartitionKey partitionKey)> items,
        ReadManyRequestOptions? readManyRequestOptions = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<ItemResponse<T>> PatchItemAsync<T>(
        string id,
        PartitionKey partitionKey,
        IReadOnlyList<PatchOperation> patchOperations,
        PatchItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<ResponseMessage> PatchItemStreamAsync(
        string id,
        PartitionKey partitionKey,
        IReadOnlyList<PatchOperation> patchOperations,
        PatchItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<ResponseMessage> DeleteItemStreamAsync(
        string id,
        PartitionKey partitionKey,
        ItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<ItemResponse<T>> DeleteItemAsync<T>(
        string id,
        PartitionKey partitionKey,
        ItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override FeedIterator GetItemQueryStreamIterator(
        QueryDefinition queryDefinition,
        string? continuationToken = null,
        QueryRequestOptions? requestOptions = null)
    {
        throw new NotImplementedException();
    }

    public override FeedIterator<T> GetItemQueryIterator<T>(
        QueryDefinition queryDefinition,
        string? continuationToken = null,
        QueryRequestOptions? requestOptions = null)
    {
        throw new NotImplementedException();
    }

    public override FeedIterator GetItemQueryStreamIterator(
        string? queryText = null,
        string? continuationToken = null,
        QueryRequestOptions? requestOptions = null)
    {
        throw new NotImplementedException();
    }

    public override FeedIterator<T> GetItemQueryIterator<T>(
        string? queryText = null,
        string? continuationToken = null,
        QueryRequestOptions? requestOptions = null)
    {
        throw new NotImplementedException();
    }

    public override FeedIterator GetItemQueryStreamIterator(
        FeedRange feedRange,
        QueryDefinition queryDefinition,
        string continuationToken,
        QueryRequestOptions? requestOptions = null)
    {
        throw new NotImplementedException();
    }

    public override FeedIterator<T> GetItemQueryIterator<T>(
        FeedRange feedRange,
        QueryDefinition queryDefinition,
        string? continuationToken = null,
        QueryRequestOptions? requestOptions = null)
    {
        throw new NotImplementedException();
    }

    public override IOrderedQueryable<T> GetItemLinqQueryable<T>(
        bool allowSynchronousQueryExecution = false,
        string? continuationToken = null,
        QueryRequestOptions? requestOptions = null,
        CosmosLinqSerializerOptions? linqSerializerOptions = null)
    {
        throw new NotImplementedException();
    }

    public override ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilder<T>(
        string processorName,
        ChangesHandler<T> onChangesDelegate)
    {
        throw new NotImplementedException();
    }

    public override ChangeFeedProcessorBuilder GetChangeFeedEstimatorBuilder(
        string processorName,
        ChangesEstimationHandler estimationDelegate,
        TimeSpan? estimationPeriod = null)
    {
        throw new NotImplementedException();
    }

    public override ChangeFeedEstimator GetChangeFeedEstimator(
        string processorName,
        Container leaseContainer)
    {
        throw new NotImplementedException();
    }

    public override TransactionalBatch CreateTransactionalBatch(
        PartitionKey partitionKey)
    {
        throw new NotImplementedException();
    }

    public override Task<IReadOnlyList<FeedRange>> GetFeedRangesAsync(
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override FeedIterator GetChangeFeedStreamIterator(
        ChangeFeedStartFrom changeFeedStartFrom,
        ChangeFeedMode changeFeedMode,
        ChangeFeedRequestOptions? changeFeedRequestOptions = null)
    {
        throw new NotImplementedException();
    }

    public override FeedIterator<T> GetChangeFeedIterator<T>(
        ChangeFeedStartFrom changeFeedStartFrom,
        ChangeFeedMode changeFeedMode,
        ChangeFeedRequestOptions? changeFeedRequestOptions = null)
    {
        throw new NotImplementedException();
    }

    public override ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilder<T>(
        string processorName,
        ChangeFeedHandler<T> onChangesDelegate)
    {
        throw new NotImplementedException();
    }

    public override ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilderWithManualCheckpoint<T>(
        string processorName,
        ChangeFeedHandlerWithManualCheckpoint<T> onChangesDelegate)
    {
        throw new NotImplementedException();
    }

    public override ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilder(
        string processorName,
        ChangeFeedStreamHandler onChangesDelegate)
    {
        throw new NotImplementedException();
    }

    public override ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilderWithManualCheckpoint(
        string processorName,
        ChangeFeedStreamHandlerWithManualCheckpoint onChangesDelegate)
    {
        throw new NotImplementedException();
    }

    public override string Id { get; }
    public override Database Database { get; }
    public override Conflicts Conflicts { get; }
    public override Scripts Scripts { get; }

    #endregion
}