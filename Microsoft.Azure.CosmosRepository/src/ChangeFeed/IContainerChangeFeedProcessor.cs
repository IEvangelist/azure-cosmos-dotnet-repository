// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.CosmosRepository.ChangeFeed
{
    internal interface IContainerChangeFeedProcessor
    {
        Task StartAsync(CancellationToken cancellationToken);

        Task StopAsync(CancellationToken cancellationToken);

        public IReadOnlyList<Type> ItemTypes { get; }
    }
}