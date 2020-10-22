// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Azure.CosmosRepository
{
    /// <inheritdoc/>
    class DefaultRepositoryFactory : IRepositoryFactory
    {
        readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Constructor for the default respositroy factory.
        /// </summary>
        /// <param name="serviceProvider"></param>
        public DefaultRepositoryFactory(IServiceProvider serviceProvider) =>
            _serviceProvider = serviceProvider
                ?? throw new ArgumentNullException(
                    nameof(serviceProvider),
                    "A service provider instance is required.");

        /// <inheritdoc/>
        public IRepository<TItem> RepositoryOf<TItem>()
            where TItem : Item =>
            _serviceProvider.GetRequiredService<IRepository<TItem>>();
    }
}
