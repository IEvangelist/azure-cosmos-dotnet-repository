// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Aspire.Microsoft.Azure.CosmosRepository.Tests.Fixtures;

public class CosmosDatabaseFixture : IAsyncDisposable
{
    public CosmosDatabaseFixture()
    {
        DockerCosmosDatabase.StartAsync().Wait();
    }

    public async ValueTask DisposeAsync()
    {
        await DockerCosmosDatabase.StopAsync();
    }
}