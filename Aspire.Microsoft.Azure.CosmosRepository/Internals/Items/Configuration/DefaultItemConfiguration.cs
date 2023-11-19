// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Aspire.Microsoft.Azure.CosmosRepository.Items;
using Aspire.Microsoft.Azure.CosmosRepository.Items.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aspire.Microsoft.Azure.CosmosRepository.Internals.Items.Configuration;

public class DefaultItemConfiguration(IServiceProvider serviceProvider) : IItemConfiguration
{
    public ICosmosItemConfiguration<TItem> For<TItem>() where TItem : IItem =>
        serviceProvider.GetRequiredService<ICosmosItemConfiguration<TItem>>();
}