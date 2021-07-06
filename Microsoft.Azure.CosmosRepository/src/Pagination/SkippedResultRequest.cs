// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.CosmosRepository.Pagination
{
    /// <inheritdoc />
    public class SkippedResultRequest : ISkippedResultRequest
    {
        /// <inheritdoc />
        public int SkipCount { get; set; }
    }
}
