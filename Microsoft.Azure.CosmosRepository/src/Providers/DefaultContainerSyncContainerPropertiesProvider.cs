// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Linq;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.CosmosRepository.Providers
{
    /// <inheritdoc />
    class DefaultContainerSyncContainerPropertiesProvider : ICosmosContainerSyncContainerPropertiesProvider
    {
        private readonly IOptions<RepositoryOptions> _options;

        public DefaultContainerSyncContainerPropertiesProvider(IOptions<RepositoryOptions> options) => _options = options;

        /// <inheritdoc />
        public bool GetWhetherToSyncContainerProperties<TItem>() where TItem : IItem
        {
            RepositoryOptions repositoryOptions = _options.Value;

            if (repositoryOptions.SyncAllContainerProperties)
            {
                return true;
            }

            return repositoryOptions.GetContainerOptions<TItem>()?.SyncContainerProperties ?? false;
        }
    }
}