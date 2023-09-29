// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository;

/// <summary>
/// A factory abstraction for a component that can
/// create <see cref="IRepository{TItem}"/> instances.
/// </summary>
public interface IRepositoryFactory
{
    /// <summary>
    /// Gets an <see cref="IRepository{TItem}"/> instance for the
    /// given <typeparamref name="TItem"/> type.
    /// </summary>
    /// <typeparam name="TItem">The item type that corresponds to the repository.</typeparam>
    /// <returns>An <see cref="IRepository{TItem}"/> of <typeparamref name="TItem"/>.</returns>
    IRepository<TItem> RepositoryOf<TItem>() where TItem : class, IItem;
}