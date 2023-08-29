// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.Services;

public class DefaultCosmosContainerSyncServiceTests
{
    Mock<ICosmosContainerService> _containerService = new();
    readonly DefaultCosmosContainerSyncService _syncService;

    public DefaultCosmosContainerSyncServiceTests()
    {
        _syncService = new DefaultCosmosContainerSyncService(_containerService.Object);
    }

    [Fact]
    public async Task SyncContainerPropertiesAsync_Item_SyncsItem()
    {
        await _syncService.SyncContainerPropertiesAsync<TestItemWithEtag>();

        _containerService.Verify(o => o.GetContainerAsync<TestItemWithEtag>(true));
    }
}