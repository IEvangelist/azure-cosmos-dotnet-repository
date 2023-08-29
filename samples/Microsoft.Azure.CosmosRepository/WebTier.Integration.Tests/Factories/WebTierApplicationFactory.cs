// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace WebTier.Integration.Tests.Factories;

public class WebTierApplicationFactory : WebApplicationFactory<Startup>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services => services
                .RemoveCosmosRepositories()
                .AddInMemoryCosmosRepository());
        base.ConfigureWebHost(builder);
    }
}