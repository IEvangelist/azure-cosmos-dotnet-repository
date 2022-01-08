// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Microsoft.Azure.CosmosRepository.ChangeFeed.Processors;

namespace Microsoft.Azure.CosmosRepository.ChangeFeed.Providers
{
    internal interface IChangeFeedContainerProcessorProvider
    {
        List<IChangeFeedContainerProcessor> GetProcessors();
    }
}