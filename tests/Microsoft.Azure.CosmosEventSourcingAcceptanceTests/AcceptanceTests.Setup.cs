// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosEventSourcing.Extensions;
using Microsoft.Azure.CosmosEventSourcing.Stores;
using Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Items;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Azure.CosmosEventSourcingAcceptanceTests;

public partial class AcceptanceTests
{
    private const string DatabaseName = "cosmos-event-sourcing-tests";
    private readonly IServiceProvider _provider;
    private readonly ILogger<AcceptanceTests> _logger;
    private readonly IEventStore<TodoListEventItem> _todoListItemEventStore;

    private string ConnectionString =>
        Environment.GetEnvironmentVariable("CosmosConnectionString") ??
        throw new ArgumentNullException(
            nameof(ConnectionString),
            "The CosmosConnectionString ENV var must be set for these tests");

    public AcceptanceTests(ITestOutputHelper outputHelper)
    {
        IServiceCollection services = new ServiceCollection();

        services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());

        services.AddLogging(options =>
        {
            options.ClearProviders();
            options.AddXUnit(outputHelper, loggerOptions =>
            {
                loggerOptions.Filter = (s, _) =>
                    s is null || !s.StartsWith("System.Net");
            });

            options.SetMinimumLevel(LogLevel.Debug);
        });

        services.AddCosmosEventSourcing(builder =>
        {
            builder.AddCosmosRepository(options =>
            {
                options.CosmosConnectionString = ConnectionString;
                options.DatabaseId = DatabaseName;

                options.ContainerBuilder
                    .ConfigureEventItemStore<TodoListEventItem>("todo-list-events");
            });

            builder.AddDomainEventTypes(typeof(AcceptanceTests).Assembly);
        });

        _provider = services.BuildServiceProvider();
        _logger = _provider.GetRequiredService<ILogger<AcceptanceTests>>();
        _todoListItemEventStore = _provider.GetRequiredService<IEventStore<TodoListEventItem>>();
    }

    [Fact]
    public async Task CosmosEventSourcingTest()
    {
        ICosmosClientProvider clientProvider =
            _provider.GetRequiredService<ICosmosClientProvider>();

        await clientProvider
            .UseClientAsync(x =>
                x.DeleteDatabaseIfExistsAsync(DatabaseName, _logger));

        try
        {
            await Execute();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Test failed");
            throw;
        }
        finally
        {
            await clientProvider
                .UseClientAsync(x =>
                    x.DeleteDatabaseIfExistsAsync(DatabaseName, _logger));
        }
    }
}