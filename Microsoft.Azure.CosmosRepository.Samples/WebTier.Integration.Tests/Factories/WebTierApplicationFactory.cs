// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WebTier.Integration.Tests.Factories
{
    public class WebTierApplicationFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services
                    .RemoveCosmosRepositories()
                    .AddInMemoryCosmosRepository();
            });
            base.ConfigureWebHost(builder);
        }
    }
}