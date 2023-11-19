// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Aspire.Microsoft.Azure.CosmosRepository.Items;
using Aspire.Microsoft.Azure.CosmosRepository.Items.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aspire.Microsoft.Azure.CosmosRepository.Internals.Builders;

public class CosmosRepositoryBuilder(IHostApplicationBuilder hostApplicationBuilder) : ICosmosRepositoryBuilder
{
    private readonly IServiceCollection _services = hostApplicationBuilder.Services;

    public IHostApplicationBuilder HostApplicationBuilder { get; } = hostApplicationBuilder;

    public ICosmosRepositoryBuilder AddItemConfiguration<TItem, TConfiguration>()
        where TItem : IItem
        where TConfiguration : class, ICosmosItemConfiguration<TItem>
    {
        _services.AddSingleton<ICosmosItemConfiguration<TItem>, TConfiguration>();
        return this;
    }
}