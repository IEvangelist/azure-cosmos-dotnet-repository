// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosEventSourcing.Extensions;
using Microsoft.Azure.CosmosEventSourcing.Stores;
using Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Items;
using Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Projections;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.ChangeFeed;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Azure.CosmosEventSourcingAcceptanceTests;

[Trait("Category", "Acceptance")]
[Trait("Type", "CosmosEventSourcing")]
public partial class AcceptanceTests
{
    private const string DatabaseName = "cosmos-event-sourcing-tests";
    private readonly IServiceProvider _provider;
    private readonly ILogger<AcceptanceTests> _logger;
    private readonly IEventStore<TodoListEventItem> _todoListItemEventStore;
    private readonly IReadOnlyRepository<TodoListItem> _todoListItemRepository;
    private readonly IChangeFeedService _changeFeedService;
    private readonly IReadOnlyRepository<TodoCosmosItem> _todoItemsRepository;

    private string ConnectionString =>
        Environment.GetEnvironmentVariable("CosmosConnectionString") ??
        throw new ArgumentNullException(
            nameof(ConnectionString),
            "The CosmosConnectionString ENV var must be set for these tests");

    private string ConnectionStringDynamic =>
        Environment.GetEnvironmentVariable("CosmosConnectionString") is string connectionString
            ? connectionString
            : throw new ArgumentNullException(
                nameof(ConnectionString),
                "The CosmosConnectionString ENV var must be set for these tests");

    public AcceptanceTests(ITestOutputHelper outputHelper)
    {
        IServiceCollection services = new ServiceCollection();

        services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());

        services.AddLogging(options =>
        {
            const string ns = $""""
                {nameof(Microsoft)}.{nameof(Azure)}.{nameof(CosmosRepository)}
                """";

            options.ClearProviders();
            options.AddXUnit(outputHelper,
                loggerOptions => loggerOptions.Filter = (s, _) =>
                    s is null || (!s.StartsWith("System.Net") && !s.StartsWith(ns)));

            options.SetMinimumLevel(LogLevel.Debug);
        });

        services.AddCosmosEventSourcing(builder =>
        {
            builder.AddCosmosRepository(options =>
            {
                options.CosmosConnectionString = ConnectionString;
                options.DatabaseId = DatabaseName;

                options.ContainerBuilder
                    .ConfigureEventItemStore<TodoListEventItem>("todo-list-events")
                    .ConfigureProjectionStore<TodoListItem>("projections")
                    .ConfigureProjectionStore<TodoCosmosItem>("projections");
            });

            builder.AddDomainEventTypes(typeof(AcceptanceTests).Assembly);

            builder.AddDefaultDomainEventProjection<TodoListEventItem, TodoItemProjectionsKey>(options =>
            {
                options.ProcessorName = "event-based-projections";
                options.InstanceName = Environment.MachineName;
                options.PollInterval = TimeSpan.FromMilliseconds(100);
            });

            builder.AddDefaultDomainEventProjection<TodoListEventItem, CompletedProjectionsKey>(options =>
            {
                options.ProcessorName = "completed-event-based-projections";
                options.InstanceName = Environment.MachineName;
                options.PollInterval = TimeSpan.FromMilliseconds(100);
            });

            builder.AddEventItemProjection<TodoListEventItem, DefaultKey, TodoListEventItemProjection>(
                options =>
                {
                    options.ProcessorName = "default-projections";
                    options.InstanceName = Environment.MachineName;
                    options.PollInterval = TimeSpan.FromMilliseconds(100);
                });

            builder.AddDomainEventProjectionHandlers(typeof(AcceptanceTests).Assembly);
        });

        _provider = services.BuildServiceProvider();
        _logger = _provider.GetRequiredService<ILogger<AcceptanceTests>>();
        _todoListItemEventStore = _provider.GetRequiredService<IEventStore<TodoListEventItem>>();
        _todoListItemRepository = _provider.GetRequiredService<IReadOnlyRepository<TodoListItem>>();
        _todoItemsRepository = _provider.GetRequiredService<IReadOnlyRepository<TodoCosmosItem>>();
        _changeFeedService = _provider.GetRequiredService<IChangeFeedService>();
    }

    [Fact(Skip = "In discussing this with Bill, we've decided that this might not be reliable enough to justify having it be a release gate.")]
    public async Task CosmosEventSourcingTest()
    {
        ICosmosClientProvider clientProvider =
            _provider.GetRequiredService<ICosmosClientProvider>();

        await clientProvider
            .UseClientAsync(x =>
                x.DeleteDatabaseIfExistsAsync(DatabaseName, _logger));

        await Policy
            .Handle<CosmosException>(static x =>
                x.StatusCode is HttpStatusCode.NotFound)
            .RetryAsync(5, (_, i) =>
                _logger.LogInformation("Attempting to start change feed service attempt {Attempt}", i))
            .ExecuteAsync(() =>
                _changeFeedService.StartAsync(default));

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
            await _changeFeedService.StopAsync();
            await clientProvider
                .UseClientAsync(x =>
                    x.DeleteDatabaseIfExistsAsync(DatabaseName, _logger));
        }
    }
}