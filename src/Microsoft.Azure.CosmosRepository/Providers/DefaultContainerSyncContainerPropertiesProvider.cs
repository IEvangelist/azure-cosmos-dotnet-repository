// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Providers;

/// <inheritdoc />
class DefaultContainerSyncContainerPropertiesProvider(IOptions<RepositoryOptions> options) : ICosmosContainerSyncContainerPropertiesProvider
{

    /// <inheritdoc />
    public bool GetWhetherToSyncContainerProperties<TItem>() where TItem : IItem =>
        GetWhetherToSyncContainerProperties(typeof(TItem));

    public bool GetWhetherToSyncContainerProperties(Type itemType)
    {
        RepositoryOptions repositoryOptions = options.Value;

        if (repositoryOptions.SyncAllContainerProperties)
        {
            return true;
        }

        return repositoryOptions.GetContainerOptions(itemType)?.SyncContainerProperties ?? false;
    }
}