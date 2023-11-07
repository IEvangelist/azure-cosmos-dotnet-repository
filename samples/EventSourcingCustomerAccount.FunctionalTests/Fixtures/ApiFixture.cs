// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using MartinCostello.Logging.XUnit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Azure.CosmosEventSourcing.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace EventSourcingCustomerAccount.FunctionalTests.Fixtures;

public class ApiFixture : WebApplicationFactory<Program>, ITestOutputHelperAccessor
{
    public ITestOutputHelper? OutputHelper { get; set; }

    public HttpClient HttpClient { get; set; }

    public ApiFixture()
    {
        HttpClient = CreateClient();
    }

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.ConfigureLogging(loggingBuilder => loggingBuilder.ClearProviders().AddXUnit(this));

        // Override the default configuration to use an in-memory event store.
        builder.ConfigureServices(
            services => services
                .AddInMemoryCosmosEventSourcing()
                .AddInMemoryCosmosRepository());
    }
}