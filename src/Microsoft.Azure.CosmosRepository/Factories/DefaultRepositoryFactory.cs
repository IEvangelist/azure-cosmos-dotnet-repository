// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository;

/// <inheritdoc/>
/// <summary>
/// Constructor for the default repository factory.
/// </summary>
/// <param name="serviceProvider"></param>
class DefaultRepositoryFactory(IServiceProvider serviceProvider) : IRepositoryFactory
{

    /// <inheritdoc/>
    public IRepository<TItem> RepositoryOf<TItem>()
        where TItem : class, IItem =>
        serviceProvider.GetRequiredService<IRepository<TItem>>();
}
