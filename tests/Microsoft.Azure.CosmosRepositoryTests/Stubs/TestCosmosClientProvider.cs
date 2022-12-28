﻿// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Providers;

namespace Microsoft.Azure.CosmosRepositoryTests.Stubs;

internal class TestCosmosClientProvider : ICosmosClientProvider
{
    readonly CosmosClient _cosmosClient;

    public TestCosmosClientProvider(CosmosClient cosmosClient) =>
        _cosmosClient = cosmosClient;

    public Task<T> UseClientAsync<T>(Func<CosmosClient, Task<T>> consume)
        => consume(_cosmosClient) ?? throw new ArgumentNullException(nameof(consume));
}