using Azure.Core;
using CleanArchitecture.Exceptions.AspNetCore;
using EventSourcingJobsTracker.API.Requests;
using EventSourcingJobsTracker.Application;
using EventSourcingJobsTracker.Application.Infrastructure;
using EventSourcingJobsTracker.Infrastructure.Items;
using EventSourcingJobsTracker.Infrastructure.Repositories;
using Microsoft.Azure.CosmosEventSourcing.Extensions;
using Microsoft.Azure.CosmosRepository.AspNetCore.Extensions;
using Microsoft.Azure.CosmosRepository.Builders;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCleanArchitectureExceptionsHandler(options => options.ApplicationName = typeof(Program).Assembly.FullName)
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

builder.Services.AddCosmosEventSourcing(eventSourcingBuilder =>
{
    eventSourcingBuilder.AddCosmosRepository(cosmosOptions =>
    {
        cosmosOptions.DatabaseId = "jobs-list-db";
        IItemContainerBuilder containerBuilder = cosmosOptions.ContainerBuilder;
        containerBuilder.ConfigureEventItemStore<JobListEventItem>("jobs-list-events");
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
    .Produces(400)
    .WithTags("Jobs List");

app.MapGet("/api/jobs-list/{id}", (Guid id) => id)
    .Produces(200)
    .Produces(404)
    .Produces(404)
    .WithTags("Jobs List");;

app.Run();