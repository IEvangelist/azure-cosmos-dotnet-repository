// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Pagination
{
    /// <summary>
    /// This interface is defined to standardize to request a skipped result.
    /// </summary>
    public interface ISkippedResultRequest
    {
        /// <inheritdoc />
        int SkipCount { get; set; }
    }
}
