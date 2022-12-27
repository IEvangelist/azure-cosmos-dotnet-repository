// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Providers;

class DefaultRepositoryExpressionProvider : IRepositoryExpressionProvider
{
    private readonly ICosmosItemConfigurationProvider _itemConfigurationProvider;

    public DefaultRepositoryExpressionProvider(ICosmosItemConfigurationProvider itemConfigurationProvider) =>
        _itemConfigurationProvider = itemConfigurationProvider;

    public Expression<Func<TItem, bool>> Build<TItem>(Expression<Func<TItem, bool>> predicate)
        where TItem : IItem
    {
        ItemConfiguration options = _itemConfigurationProvider.GetItemConfiguration<TItem>();

        return options.UseStrictTypeChecking ? predicate.Compose(Default<TItem>(), Expression.AndAlso) : predicate;
    }

    public Expression<Func<TItem, bool>> Default<TItem>() where TItem : IItem =>
        item => !item.Type.IsDefined() || item.Type == typeof(TItem).Name;

    public TItem CheckItem<TItem>(TItem item) where TItem : IItem
    {
        ItemConfiguration options = _itemConfigurationProvider.GetItemConfiguration<TItem>();

        if (options.UseStrictTypeChecking)
        {
            return item is { Type: { Length: 0 } } || item.Type == typeof(TItem).Name
                ? item
                : throw new MissMatchedTypeDiscriminatorException(typeof(TItem).Name, item.Type);
        }

        return item;
    }
}