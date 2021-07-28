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
        public ConcurrentDictionary<Type, ContainerOptions> Options { get; } = new();


        public IItemContainerBuilder Configure<TItem>(Action<ContainerOptions> configure) where TItem : IItem
        {
            if (configure is null) throw new ArgumentNullException(nameof(configure));

            ContainerOptions options = new(typeof(TItem));

            configure(options);

            Options.TryAdd(typeof(TItem), options);

            return this;
        }
    }
}