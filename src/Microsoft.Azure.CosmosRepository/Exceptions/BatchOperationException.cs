// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Exceptions;

/// <summary>
/// Details an error when performing a batch operation.
/// </summary>
/// <remarks>Creates <see cref="BatchOperationException"/>.</remarks>
public class BatchOperationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BatchOperationException"/> class.
    /// </summary>
    /// <param name="response">The response from the batch operation.</param>
    public BatchOperationException(TransactionalBatchResponse response)
        : this(response, CreateMessage(response))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BatchOperationException"/> class.
    /// </summary>
    /// <param name="response">The response from the batch operation.</param>
    /// <param name="message">The exception message.</param>
    protected BatchOperationException(TransactionalBatchResponse response, string message)
        : base(message)
    {
        if (response is null)
        {
            throw new ArgumentNullException(nameof(response));
        }

        Response = response;
    }

    /// <summary>
    /// The response from the batch operation.
    /// </summary>
    public TransactionalBatchResponse Response { get; }

    /// <summary>
    /// The status code returned from the <see cref="TransactionalBatchResponse"/>.
    /// </summary>
    public HttpStatusCode StatusCode => Response.StatusCode;

    private static string CreateMessage(TransactionalBatchResponse response)
    {
        if (response is null)
        {
            throw new ArgumentNullException(nameof(response));
        }

        return $"Failed to execute batch operation. Status: {response.StatusCode}";
    }
}

/// <summary>
/// Details an error when performing a batch operation for a given <typeparamref name="TItem"/>.
/// </summary>
/// <typeparam name="TItem">The item type for the batch.</typeparam>
/// <remarks>Creates <see cref="BatchOperationException{TItem}"/>.</remarks>
public class BatchOperationException<TItem>(TransactionalBatchResponse response)
    : BatchOperationException(response, $"Failed to execute the batch operation for {typeof(TItem).Name}")
    where TItem : IItem;
