// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Azure.CosmosRepository.Options;

namespace Microsoft.Azure.CosmosRepository.Builders
{
    /// <inheritdoc/>
    internal class DefaultItemBuilder : IItemBuilder
    {
        private readonly List<ContainerOptionsBuilder> _containerItemOptions = new();

        public IReadOnlyList<ContainerOptionsBuilder> ContainerItemOptions => _containerItemOptions;

        public IItemBuilder ConfigureContainer<TItem>(Action<ContainerOptionsBuilder> containerOptions) where TItem : IItem
        {
            if (containerOptions is null) throw new ArgumentNullException(nameof(containerOptions));

            ContainerOptionsBuilder optionsBuilder = new(typeof(TItem));

            containerOptions(optionsBuilder);

            _containerItemOptions.Add(optionsBuilder);

            return this;
        }

        public IItemBuilder ConfigureOptions<TItem>(Action<ItemOptionsBuilder> metadataBuilder) where TItem : IItem
        {
            throw new NotImplementedException();
        }
    }
}