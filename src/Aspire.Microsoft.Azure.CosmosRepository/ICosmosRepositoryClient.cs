// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;

namespace Aspire.Microsoft.Azure.CosmosRepository;

/// <summary>
/// This is intended to wrap the <see cref="CosmosClient"/> and provide a way to mock it easily in testing.
/// </summary>
public interface ICosmosRepositoryClient
{
    Container GetContainer(
        string databaseId,
        string containerId);

    Task<Container> CreateContainerAndDatabaseIfNotExistsAsync (
        string databaseId,
        string containerId,
        string partitionKeyPath);
}