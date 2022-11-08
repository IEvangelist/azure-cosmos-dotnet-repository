// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Azure.CosmosRepository;

/// <inheritdoc/>
class DefaultRepositoryFactory : IRepositoryFactory
{
    readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Constructor for the default repository factory.
    /// </summary>
    /// <param name="serviceProvider"></param>
    public DefaultRepositoryFactory(IServiceProvider serviceProvider) =>
        _serviceProvider = serviceProvider;

    /// <inheritdoc/>
    public IRepository<TItem> RepositoryOf<TItem>()
        where TItem : class, IItem =>
        _serviceProvider.GetRequiredService<IRepository<TItem>>();
}
