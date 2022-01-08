// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;

namespace Microsoft.Azure.CosmosRepository.ChangeFeed.Processors
{
    internal interface IChangeFeedContainerProcessor
    {
        Task StartAsync();

        Task StopAsync();
    }
}