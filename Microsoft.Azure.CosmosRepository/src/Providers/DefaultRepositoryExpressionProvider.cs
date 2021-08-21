// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq.Expressions;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.CosmosRepository.Extensions;

namespace Microsoft.Azure.CosmosRepository.Providers
{
    class DefaultRepositoryExpressionProvider : IRepositoryExpressionProvider
    {
        public Expression<Func<TItem, bool>> Build<TItem>(Expression<Func<TItem, bool>> predicate) where TItem : IItem =>
            predicate.Compose(item => !item.Type.IsDefined() || item.Type == typeof(TItem).Name, Expression.AndAlso);
    }
}