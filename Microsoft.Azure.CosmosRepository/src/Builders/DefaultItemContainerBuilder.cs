// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Azure.CosmosRepository.Options;

namespace Microsoft.Azure.CosmosRepository.Builders
{
    /// <inheritdoc/>
    internal class DefaultItemContainerBuilder : IItemContainerBuilder
    {
        private readonly List<ContainerOptions> _options = new();

        public IReadOnlyList<ContainerOptions> Options => _options;

        public IItemContainerBuilder Configure<TItem>(Action<ContainerOptions> containerOptions) where TItem : IItem
        {
            if (containerOptions is null) throw new ArgumentNullException(nameof(containerOptions));

            ContainerOptions options = new(typeof(TItem));

            containerOptions(options);

            _options.Add(options);

            return this;
        }
    }
}