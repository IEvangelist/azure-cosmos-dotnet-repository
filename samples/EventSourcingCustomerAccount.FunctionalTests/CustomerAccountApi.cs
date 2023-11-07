// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Azure.CosmosEventSourcing.Extensions;
using Microsoft.Azure.CosmosRepository.AspNetCore.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventSourcingCustomerAccount.FunctionalTests;

public class CustomerAccountApi : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        // Override the default configuration to use an in-memory event store.
        builder.ConfigureServices(
            services =>
            {
                services
                    .AddInMemoryCosmosEventSourcing()
                    .AddInMemoryCosmosRepository();

                services.RemoveCosmosRepositoryChangeFeedHostedService();
            });

        builder.ConfigureAppConfiguration(
            config =>
            {
                var dictionary = new Dictionary<string, string?>
                {
                    ["RepositoryOptions:CosmosConnectionString"] = "AccountEndpoint=http://foo.net;AccountKey=Zm9v"
                };

                config.AddInMemoryCollection(dictionary);
            });
    }
}