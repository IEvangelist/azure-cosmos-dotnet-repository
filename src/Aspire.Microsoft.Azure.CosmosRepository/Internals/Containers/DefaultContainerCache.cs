// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Aspire.Microsoft.Azure.CosmosRepository.Containers;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Aspire.Microsoft.Azure.CosmosRepository.Internals.Containers;

public class DefaultContainerCache(
    IOptionsMonitor<AzureCosmosDbRepositorySettings> optionsMonitor,
    CosmosClient cosmosClient) : IContainerCache
{
    private readonly AzureCosmosDbRepositorySettings _settings = optionsMonitor.CurrentValue;
    public Task<Container> GetContainerAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        //TODO: do we NEED to cache these like we did before? Or does the SDK do it?
        Container container = cosmosClient.GetContainer(_settings.DatabaseName, name);
        return Task.FromResult(container);
    }
}