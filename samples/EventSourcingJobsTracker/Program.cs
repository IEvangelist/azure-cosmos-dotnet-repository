using CleanArchitecture.Exceptions.AspNetCore;
using EventSourcingJobsTracker.API.DTOs;
using EventSourcingJobsTracker.API.Requests;
using EventSourcingJobsTracker.Application.Infrastructure;
using EventSourcingJobsTracker.Application.Services;
using EventSourcingJobsTracker.Core.Aggregates;
using EventSourcingJobsTracker.Infrastructure.Items;
using EventSourcingJobsTracker.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
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
    .AddSingleton<IJobListService, DefaultJobListService>()
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
    .WithTags("Jobs Lists");

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
    .WithTags("Jobs");

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
    .WithTags("Jobs");

app.MapGet(
        "/api/jobs-list/{id}",
        async (
            Guid id,
            string username,
            [FromServices] IJobTrackerReadService readService) =>
        {
            JobsListDto? jobsList = await readService.FindJobsListAsync(id, username);

            return jobsList is null
                ? Results.NoContent()
                : Results.Ok(jobsList);
        })
    .Produces(200)
    .Produces(204)
    .WithTags("Jobs Lists");

app.MapGet(
        "/api/jobs-list/",
        async (
            string username,
            [FromServices] IJobTrackerReadService readService) =>
        {
            IEnumerable<JobsListDto> jobLists = await readService.FindJobsListAsync(username);

            return jobLists.Any()
                ? Results.Ok(jobLists)
                : Results.NoContent();
        })
    .Produces(200)
    .Produces(204)
    .WithTags("Jobs Lists");

app.MapGet(
        "/api/jobs-list/jobs/",
        async (
            Guid jobListId,
            [FromServices] IJobTrackerReadService readService) =>
        {
            IEnumerable<JobDto> jobs = await readService.FindJobsForJobsListAsync(jobListId);

            return jobs.Any()
                ? Results.Ok(jobs)
                : Results.NoContent();
        })
    .Produces(200)
    .Produces(204)
    .WithTags("Jobs");

app.Run();