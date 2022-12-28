// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using CleanArchitecture.Exceptions.AspNetCore;
using EventSourcingJobsTracker.API.DTOs;
using EventSourcingJobsTracker.API.Requests;
using EventSourcingJobsTracker.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventSourcingJobsTracker.Endpoints;

public static class JobEndpoints
{
    public const string Tag = "Jobs";
    public static IEndpointRouteBuilder MapJobEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(
                "/api/jobs-list/jobs/",
                async (
                    CreateJob request,
                    IJobsTrackerService service) =>
                {
                    (Guid jobListId, var title, DateTime due) = request;

                    await service.AddJobAsync(jobListId, title, due);

                    return Results.Ok();
                })
            .Accepts<CreateJob>("application/json")
            .Produces(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(404)
            .WithTags(Tag);

        builder.MapPut(
                "/api/jobs-list/jobs/complete",
                async (
                    CompleteJob request,
                    IJobsTrackerService service) =>
                {
                    (Guid jobListId, Guid jobId) = request;

                    await service.CompleteJobAsync(jobListId, jobId);

                    return Results.Ok();
                })
            .Accepts<CompleteJob>("application/json")
            .Produces(200)
            .Produces<ErrorResponse>(400)
            .Produces<ErrorResponse>(404)
            .WithTags(Tag);

        builder.MapGet(
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
            .WithTags(Tag);

        return builder;
    }
}