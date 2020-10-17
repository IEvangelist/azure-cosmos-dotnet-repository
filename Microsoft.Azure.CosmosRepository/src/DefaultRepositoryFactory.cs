// Copyright (c) IEvangelist. All rights reserved. Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository
{
    using System;
    using Microsoft.Extensions.DependencyInjection;

    /// <inheritdoc />
    internal class DefaultRepositoryFactory : IRepositoryFactory
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Constructor for the default respositroy factory.
        /// </summary>
        /// <param name="serviceProvider"></param>
        public DefaultRepositoryFactory(IServiceProvider serviceProvider) =>
            this._serviceProvider = serviceProvider
                ?? throw new ArgumentNullException(
                    nameof(serviceProvider),
                    "A service provider instance is required.");

        /// <inheritdoc />
        public IRepository<TItem> RepositoryOf<TItem>()
            where TItem : Item =>
            this._serviceProvider.GetRequiredService<IRepository<TItem>>();
    }
}
