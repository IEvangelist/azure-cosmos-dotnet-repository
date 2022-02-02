// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq.Expressions;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.CosmosRepository.Extensions;
using Microsoft.Azure.CosmosRepository.Options;

namespace Microsoft.Azure.CosmosRepository.Providers
{
    class DefaultRepositoryExpressionProvider : IRepositoryExpressionProvider
    {
        private readonly ICosmosItemConfigurationProvider _itemConfigurationProvider;

        public DefaultRepositoryExpressionProvider(ICosmosItemConfigurationProvider itemConfigurationProvider) =>
            _itemConfigurationProvider = itemConfigurationProvider;

        public Expression<Func<TItem, bool>>? Build<TItem>(Expression<Func<TItem, bool>>? predicate = null)
            where TItem : IItem
        {
            ItemConfiguration options = _itemConfigurationProvider.GetItemConfiguration<TItem>();

            if (options.UseStrictTypeChecking)
            {
                return predicate is null
                    ? item => !item.Type.IsDefined() || item.Type == typeof(TItem).Name
                    : predicate.Compose(item => !item.Type.IsDefined() || item.Type == typeof(TItem).Name,
                        Expression.AndAlso);
            }

            return predicate;
        }

        public TItem? CheckItem<TItem>(TItem item) where TItem : IItem
        {
            ItemConfiguration options = _itemConfigurationProvider.GetItemConfiguration<TItem>();

            if (options.UseStrictTypeChecking)
            {
                return item is {Type: {Length: 0}} || item.Type == typeof(TItem).Name ? item : default;
            }

            return item;
        }
    }
}