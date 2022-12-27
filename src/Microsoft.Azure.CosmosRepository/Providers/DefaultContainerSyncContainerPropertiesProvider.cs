// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Providers;

/// <inheritdoc />
class DefaultContainerSyncContainerPropertiesProvider : ICosmosContainerSyncContainerPropertiesProvider
{
    private readonly IOptions<RepositoryOptions> _options;

    public DefaultContainerSyncContainerPropertiesProvider(IOptions<RepositoryOptions> options) => _options = options;

    /// <inheritdoc />
    public bool GetWhetherToSyncContainerProperties<TItem>() where TItem : IItem =>
        GetWhetherToSyncContainerProperties(typeof(TItem));

    public bool GetWhetherToSyncContainerProperties(Type itemType)
    {
        RepositoryOptions repositoryOptions = _options.Value;

        if (repositoryOptions.SyncAllContainerProperties)
        {
            return true;
        }

        return repositoryOptions.GetContainerOptions(itemType)?.SyncContainerProperties ?? false;
    }
}