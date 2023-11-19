// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;

namespace Aspire.Microsoft.Azure.CosmosRepository.Containers;

public interface IContainerCache
{
    Task<Container> GetContainerAsync(
        string name,
        CancellationToken cancellationToken = default);
}