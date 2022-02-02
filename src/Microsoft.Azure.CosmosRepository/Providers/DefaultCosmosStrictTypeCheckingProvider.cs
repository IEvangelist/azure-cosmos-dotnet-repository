// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Azure.CosmosRepository.Builders;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.CosmosRepository.Providers
{
    class DefaultCosmosStrictTypeCheckingProvider : ICosmosStrictTypeCheckingProvider
    {
        private readonly IOptions<RepositoryOptions> _options;

        public DefaultCosmosStrictTypeCheckingProvider(IOptions<RepositoryOptions> options) =>
            _options = options;

        public bool UseStrictTypeChecking<TItem>() where TItem : IItem =>
            UseStrictTypeChecking(typeof(TItem));

        public bool UseStrictTypeChecking(Type itemType)
        {
            ContainerOptionsBuilder? itemOptions = _options.Value.GetContainerOptions(itemType);

            return itemOptions?.UseStrictTypeChecking ?? true;
        }
    }
}