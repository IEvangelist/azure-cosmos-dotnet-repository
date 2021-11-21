// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository.Services;
using Microsoft.Azure.CosmosRepositoryTests.Stubs;
using Moq;
using Xunit;

namespace Microsoft.Azure.CosmosRepositoryTests.Services
{
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
            await _syncService.SyncContainerPropertiesAsync<TestItem>();

            _containerService.Verify(o => o.GetContainerAsync<TestItem>(true));
        }
    }
}