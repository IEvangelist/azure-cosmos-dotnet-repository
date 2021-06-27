// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

﻿﻿namespace Microsoft.Azure.CosmosRepository.Pagination
{
    /// <inheritdoc />
    public class PagedResultRequest : LimitedResultRequest, IPagedResultRequest
    {
        /// <inheritdoc />
        public int SkipCount { get; set; }
    }
}
