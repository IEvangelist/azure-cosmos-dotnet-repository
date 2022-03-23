// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
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
                    s is null || (!s.StartsWith("System.Net") && !s.StartsWith("Microsoft.Azure.CosmosRepository"));
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
                    .ConfigureEventItemStore<TodoListEventItem>("todo-list-events")
                    .ConfigureProjectionStore<TodoListItem>("projections")
                    .ConfigureProjectionStore<TodoCosmosItem>("projections");
            });

            builder.AddDomainEventTypes(typeof(AcceptanceTests).Assembly);

            builder.AddDefaultDomainEventProjection<TodoListEventItem, TodoItemProjectionsKey>(options =>
            {
                options.ProcessorName = "event-based-projections";
                options.InstanceName = Environment.MachineName;
                options.PollInterval = TimeSpan.FromMilliseconds(500);
            });

            builder.AddDefaultDomainEventProjection<TodoListEventItem, CompletedProjectionsKey>(options =>
            {
                options.ProcessorName = "completed-event-based-projections";
                options.InstanceName = Environment.MachineName;
                options.PollInterval = TimeSpan.FromMilliseconds(500);
            });

            builder.AddEventItemProjection<TodoListEventItem, DefaultKey, TodoListEventItemProjection>(
                options =>
                {
                    options.ProcessorName = "default-projections";
                    options.InstanceName = Environment.MachineName;
                    options.PollInterval = TimeSpan.FromMilliseconds(500);
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

    [Fact]
    public async Task CosmosEventSourcingTest()
    {
        ICosmosClientProvider clientProvider =
            _provider.GetRequiredService<ICosmosClientProvider>();

        await clientProvider
            .UseClientAsync(x =>
                x.DeleteDatabaseIfExistsAsync(DatabaseName, _logger));

        await _changeFeedService.StartAsync(default);

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