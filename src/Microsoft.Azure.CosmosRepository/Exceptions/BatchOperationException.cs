// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Net;
using Microsoft.Azure.Cosmos;

namespace Microsoft.Azure.CosmosRepository.Exceptions
{
    /// <summary>
    /// Details an error when performing a batch operation for a given TItem
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class BatchOperationException<TItem> : Exception
        where TItem : IItem
    {
        /// <summary>
        ///  The response from the batch operation.
        /// </summary>
        public TransactionalBatchResponse Response { get; }

        /// <summary>
        /// The status code return from the <see cref="TransactionalBatchResponse"/>
        /// </summary>
        public HttpStatusCode StatusCode => Response.StatusCode;

        /// <summary>
        /// Creates <see cref="BatchOperationException{TItem}"/>
        /// </summary>
        /// <param name="response"></param>
        public BatchOperationException(TransactionalBatchResponse response) : base(
            $"Failed to execute the batch operation for {typeof(TItem).Name}")
        {
            Response = response;
        }
    }
}