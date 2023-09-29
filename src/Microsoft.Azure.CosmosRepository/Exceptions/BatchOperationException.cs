// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Exceptions;

/// <summary>
/// Details an error when performing a batch operation for a given TItem
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <remarks>
/// Creates <see cref="BatchOperationException{TItem}"/>
/// </remarks>
/// <param name="response"></param>
public class BatchOperationException<TItem>(TransactionalBatchResponse response) : Exception(
    $"Failed to execute the batch operation for {typeof(TItem).Name}")
    where TItem : IItem
{
    /// <summary>
    ///  The response from the batch operation.
    /// </summary>
    public TransactionalBatchResponse Response { get; } = response;

    /// <summary>
    /// The status code return from the <see cref="TransactionalBatchResponse"/>
    /// </summary>
    public HttpStatusCode StatusCode => Response.StatusCode;
}