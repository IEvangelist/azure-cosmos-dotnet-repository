// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using CleanArchitecture.Exceptions.AspNetCore;
using EventSourcingJobsTracker.API.DTOs;
using EventSourcingJobsTracker.API.Requests;
using EventSourcingJobsTracker.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventSourcingJobsTracker.Endpoints;

public static class JobsListsEndpoints
{
    public const string Tag = "Jobs Lists";

    public static IEndpointRouteBuilder MapJobsListsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(
                "/api/jobs-list/",
                async (
                    CreateJobList request,
                    IJobsTrackerService service) =>
                {
                    (var name, var category, var username) = request;

                    Guid id = await service.CreateJobListAsync(
                        name,
                        category,
                        username);

                    return Results.Created($"api/jobs-list/{id}", id);
                })
            .Accepts<CreateJobList>("application/json")
            .Produces(201)
            .Produces<ErrorResponse>(400)
            .WithTags(Tag);

        builder.MapGet(
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
            .WithTags(Tag);

        builder.MapGet(
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
            .WithTags(Tag);

        return builder;
    }
}