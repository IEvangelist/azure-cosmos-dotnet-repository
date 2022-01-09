// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;

namespace Microsoft.Azure.CosmosRepository.ChangeFeed
{
    internal interface IContainerChangeFeedProcessor
    {
        Task StartAsync();

        Task StopAsync();
    }
}