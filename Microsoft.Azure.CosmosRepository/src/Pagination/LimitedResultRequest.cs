// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Pagination
{
    /// <inheritdoc />
    public class LimitedResultRequest : ILimitedResultRequest
    {
        /// <summary>
        /// Default value: 10.
        /// </summary>
        public static int DefaultMaxResultCount { get; set; } = 10;

        /// <inheritdoc />
        public int MaxResultCount { get; set; } = DefaultMaxResultCount;
    }
}
