// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Microsoft.Azure.CosmosRepository.ChangeFeed.Providers
{
    internal interface IChangeFeedContainerProcessorProvider
    {
        IEnumerable<IContainerChangeFeedProcessor> GetProcessors();
    }
}