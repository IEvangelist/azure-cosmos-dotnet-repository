using CleanArchitecture.Exceptions.AspNetCore;
using EventSourcingJobsTracker.Application.Infrastructure;
using EventSourcingJobsTracker.Application.Services;
using EventSourcingJobsTracker.Core.Aggregates;
using EventSourcingJobsTracker.Endpoints;
using EventSourcingJobsTracker.Infrastructure.Items;
using EventSourcingJobsTracker.Infrastructure.Repositories;
using Microsoft.Azure.CosmosEventSourcing.Extensions;
using Microsoft.Azure.CosmosRepository.AspNetCore.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string appName = typeof(Program).Assembly.FullName!;

builder.Services
    .AddCleanArchitectureExceptionsHandler(options => options.ApplicationName = appName)
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

builder.Services.AddCosmosEventSourcing(eventSourcingBuilder =>
{
    eventSourcingBuilder.AddCosmosRepository(cosmosOptions =>
    {
        cosmosOptions.DatabaseId = "jobs-list-db";
        cosmosOptions.ContainerBuilder
            .ConfigureEventItemStore<JobsListEventItem>("jobs-list-events")
            .ConfigureProjectionStore<JobsListReadItem>("projections")
            .ConfigureProjectionStore<JobItem>("projections");
    });

    eventSourcingBuilder.AddDomainEventTypes(typeof(JobsList).Assembly);
    eventSourcingBuilder.AddDomainEventProjectionHandlers(typeof(JobsList).Assembly);

    eventSourcingBuilder.AddDefaultDomainEventProjectionBuilder<JobsListEventItem>(options =>
    {
        options.InstanceName = appName;
        options.ProcessorName = Environment.MachineName;
        options.PollInterval = TimeSpan.FromSeconds(1);
    });
});

builder.Services.AddCosmosRepositoryChangeFeedHostedService();

builder.Services
    .AddSingleton<IJobsTrackerService, DefaultJobsTrackerService>()
    .AddSingleton<IJobListRepository, DefaultJobListRepository>()
    .AddSingleton<IJobTrackerReadService, DefaultJobTrackerReadService>();


WebApplication app = builder.Build();

app
    .UseCleanArchitectureExceptionsHandler()
    .UseSwagger()
    .UseSwaggerUI();

app
    .MapGet("/", () => Results.Redirect("/swagger"))
    .ExcludeFromDescription();

app.MapJobEndpoints();
app.MapJobsListsEndpoints();

app.Run();