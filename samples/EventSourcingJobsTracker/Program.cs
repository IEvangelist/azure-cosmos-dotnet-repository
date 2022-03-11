using CleanArchitecture.Exceptions.AspNetCore;
using EventSourcingJobsTracker.API.Requests;
using EventSourcingJobsTracker.Application;
using EventSourcingJobsTracker.Application.Infrastructure;
using EventSourcingJobsTracker.Core.Aggregates;
using EventSourcingJobsTracker.Infrastructure.Items;
using EventSourcingJobsTracker.Infrastructure.Repositories;
using Microsoft.Azure.CosmosEventSourcing.Extensions;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.AspNetCore.Extensions;
using Microsoft.Azure.CosmosRepository.Builders;

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
    .AddSingleton<IJobListService, DefaultJobListService>()
    .AddSingleton<IJobListRepository, DefaultJobListRepository>();


WebApplication app = builder.Build();

app
    .UseCleanArchitectureExceptionsHandler()
    .UseSwagger()
    .UseSwaggerUI();

app
    .MapGet("/", () => Results.Redirect("/swagger"))
    .ExcludeFromDescription();

app.MapPost(
        "/api/jobs-list/",
        async (
            CreateJobList request,
            IJobListService service) =>
        {
            (string name, string category, string username) = request;

            Guid id = await service.CreateJobList(
                name,
                category,
                username);

            return Results.Created($"api/jobs-list/{id}", id);
        })
    .Accepts<CreateJobList>("application/json")
    .Produces(201)
    .Produces<ErrorResponse>(400)
    .WithTags("Jobs List");

app.MapPost(
        "/api/jobs-list/jobs/",
        async (
            CreateJob request,
            IJobListService service) =>
        {
            (Guid jobListId, string? title, DateTime due) = request;

            await service.AddJob(jobListId, title, due);

            return Results.Ok();
        })
    .Accepts<CreateJob>("application/json")
    .Produces(200)
    .Produces<ErrorResponse>(400)
    .Produces<ErrorResponse>(404)
    .WithTags("Jobs List");

app.MapPut(
        "/api/jobs-list/jobs/complete",
        async (
            CompleteJob request,
            IJobListService service) =>
        {
            (Guid jobListId, Guid jobId) = request;

            await service.CompleteJob(jobListId, jobId);

            return Results.Ok();
        })
    .Accepts<CompleteJob>("application/json")
    .Produces(200)
    .Produces<ErrorResponse>(400)
    .Produces<ErrorResponse>(404)
    .WithTags("Jobs List");

app.MapGet(
        "/api/jobs-list/{id}",
        async (Guid id, string username, IReadOnlyRepository<JobsListReadItem> repository) =>
        {
            JobsListReadItem? jobsList = await repository.TryGetAsync(
                id.ToString(),
                username);

            return jobsList is null
                ? Results.NoContent()
                : Results.Ok(jobsList);
        })
    .Produces(200)
    .Produces(204)
    .WithTags("Jobs List");

app.MapGet(
        "/api/jobs-list/jobs/",
        async (Guid jobListId, IReadOnlyRepository<JobItem> repository) =>
        {
            IEnumerable<JobItem> jobs = await repository.GetAsync(x =>
                x.PartitionKey == jobListId.ToString());

            return jobs.Any()
                ? Results.Ok(jobs)
                : Results.NoContent();
        })
    .Produces(200)
    .Produces(204)
    .WithTags("Jobs List");

app.Run();