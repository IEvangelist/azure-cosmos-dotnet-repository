// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq.Expressions;

namespace Microsoft.Azure.CosmosRepository.Providers
{
    interface IRepositoryExpressionProvider
    {
        Expression<Func<TItem, bool>>? Build<TItem>(Expression<Func<TItem, bool>>? predicate) where TItem : IItem;

        TItem? CheckItem<TItem>(TItem item) where TItem : IItem;
    }
}