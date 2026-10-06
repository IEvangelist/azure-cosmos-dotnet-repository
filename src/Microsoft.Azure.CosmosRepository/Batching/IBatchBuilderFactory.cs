// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository;

/// <summary>
/// Creates <see cref="IBatchBuilder"/> instances for composing
/// transactional batches that can span multiple <see cref="IItem"/> types
/// stored in the same container.
/// </summary>
/// <example>
/// With DI, use .ctor injection to compose a cross-type batch:
/// <code language="c#">
/// <![CDATA[
/// public class ConsumingService(IBatchBuilderFactory batchBuilderFactory)
/// {
///     public ValueTask SaveAsync(SomePoco poco, OtherPoco other) =>
///         batchBuilderFactory
///             .CreateBatch(poco.PartitionKey)
///             .CreateItem(poco)
///             .CreateItem(other)
///             .ExecuteAsync();
/// }
/// ]]>
/// </code>
/// </example>
public interface IBatchBuilderFactory
{
    /// <summary>
    /// Creates a fluent batch builder for the given partition key.
    /// </summary>
    /// <param name="partitionKey">The partition key for the batch.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="partitionKey"/> is null.</exception>
    /// <returns>An <see cref="IBatchBuilder"/> for composing a batch.</returns>
    IBatchBuilder CreateBatch(string partitionKey);
}
