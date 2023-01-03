// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Providers;

interface IRepositoryExpressionProvider
{
    Expression<Func<TItem, bool>> Build<TItem>(Expression<Func<TItem, bool>> predicate) where TItem : IItem;

    Expression<Func<TItem, bool>> Default<TItem>() where TItem : IItem;

    TItem CheckItem<TItem>(TItem item) where TItem : IItem;
}