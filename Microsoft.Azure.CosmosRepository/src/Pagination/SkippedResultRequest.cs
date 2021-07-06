// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Pagination
{
    /// <inheritdoc />
    public class SkippedResultRequest : ISkippedResultRequest
    {
        /// <inheritdoc />
        public int SkipCount { get; set; }
    }
}
