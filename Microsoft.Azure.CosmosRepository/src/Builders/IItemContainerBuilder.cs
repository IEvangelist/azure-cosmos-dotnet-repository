// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Azure.CosmosRepository.Options;

namespace Microsoft.Azure.CosmosRepository.Builders
{
    /// <summary>
    /// A builder to configure a container for an item
    /// </summary>
    public interface IItemContainerBuilder
    {
        /// <summary>
        ///
        /// </summary>
        IReadOnlyList<ContainerOptions> Options { get; }

        /// <summary>
        /// Provides a <see cref="ContainerOptions"/> instance to configure a container for an item
        /// </summary>
        /// <typeparam name="TItem">The type of <see cref="IItem"/> to configure.</typeparam>
        /// <returns>Instance of <see cref="IItemContainerBuilder"/></returns>
        IItemContainerBuilder Configure<TItem>(Action<ContainerOptions> containerOptions) where TItem : IItem;
    }
}