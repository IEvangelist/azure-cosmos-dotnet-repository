// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using Microsoft.Azure.CosmosRepository.Builders;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.CosmosRepository.Providers
{
    /// <inheritdoc/>
    class DefaultCosmosContainerDefaultTimeToLiveProvider : ICosmosContainerDefaultTimeToLiveProvider
    {
        private readonly IOptions<RepositoryOptions> _options;

        /// <summary>
        /// Creates an instance of the <see cref="DefaultCosmosContainerDefaultTimeToLiveProvider"/>.
        /// </summary>
        /// <param name="options">The repository options.</param>
        public DefaultCosmosContainerDefaultTimeToLiveProvider(IOptions<RepositoryOptions> options) => _options = options;

        /// <inheritdoc/>
        public int GetDefaultTimeToLive<TItem>() where TItem : IItem
        {
            ContainerOptionsBuilder options = _options.Value.GetContainerOptions<TItem>();

            if (options is null)
            {
                return -1;
            }

            foreach (ContainerOptionsBuilder containerOptions in _options.Value.GetContainerSharedContainerOptions<TItem>())
            {
                if (containerOptions.ContainerDefaultTimeToLive != null && containerOptions.ContainerDefaultTimeToLive != options.ContainerDefaultTimeToLive)
                {
                    throw new InvalidOperationException($"The container {options.Name} has conflicting default time to live values. " +
                                                        $"({options.Type.Name}->{options.ContainerDefaultTimeToLive} vs " +
                                                        $"{containerOptions.Type.Name}->{containerOptions.ContainerDefaultTimeToLive}).");
                }
            }

            return (int)(options.ContainerDefaultTimeToLive?.TotalSeconds ?? -1);
        }
    }
}