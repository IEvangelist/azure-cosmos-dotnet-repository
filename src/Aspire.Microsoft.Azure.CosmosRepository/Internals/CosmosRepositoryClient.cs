// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;

namespace Aspire.Microsoft.Azure.CosmosRepository.Internals;

public class CosmosRepositoryClient(CosmosClient client) : ICosmosRepositoryClient
{
    public Container GetContainer(
        string databaseId,
        string containerId) =>
        client.GetContainer(
            databaseId,
            containerId);

    public async Task<Container> CreateContainerAndDatabaseIfNotExistsAsync(
        string databaseId,
        string containerId,
        string partitionKeyPath)
    {
        Database database = await client.CreateDatabaseIfNotExistsAsync(databaseId);

        Container container = await database.CreateContainerIfNotExistsAsync(
            containerId,
            partitionKeyPath);

        return container;
    }
}