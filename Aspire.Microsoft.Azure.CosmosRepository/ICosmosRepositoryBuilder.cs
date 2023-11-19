// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Aspire.Microsoft.Azure.CosmosRepository.Items;
using Aspire.Microsoft.Azure.CosmosRepository.Items.Configuration;
using Microsoft.Extensions.Hosting;

namespace Aspire.Microsoft.Azure.CosmosRepository;

public interface ICosmosRepositoryBuilder
{
    IHostApplicationBuilder HostApplicationBuilder { get; }

    ICosmosRepositoryBuilder AddItemConfiguration<TItem, TConfiguration>()
        where TConfiguration : class, ICosmosItemConfiguration<TItem>
        where TItem : IItem;
}