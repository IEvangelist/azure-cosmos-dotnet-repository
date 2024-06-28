// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using CleanArchitecture.Exceptions;
using EventSourcingJobsTracker.Core.Entities;
using EventSourcingJobsTracker.Core.Events;
using EventSourcingJobsTracker.Core.ValueObjects;
using Microsoft.Azure.CosmosEventSourcing.Aggregates;
using Microsoft.Azure.CosmosEventSourcing.Events;

namespace EventSourcingJobsTracker.Core.Aggregates;

public class JobsList : AggregateRoot
{
    private readonly List<Job> _jobs = [];

    public Guid Id { get; private set; }

    public string Name { get; private set; } = null!;

    public string Category { get; private set; } = null!;

    public string Username { get; private set; } = null!;

    public JobsList(string name, string category, string username)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException<JobsList>(
                "You must provide a name for this jobs list");
        }

        if (string.IsNullOrWhiteSpace(category))
        {
            throw new DomainException<JobsList>(
                "You must provide a category for this jobs list");
        }

        if (string.IsNullOrWhiteSpace(username))
        {
            throw new DomainException<JobsList>(
                "You must provide a username for this jobs list");
        }

        AddEvent(new JobListCreatedEvent(Guid.NewGuid(), name, category, username));
    }

    public void AddJob(string title, DateTime due)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException<JobsList>(
                "You must provide a title when creating a job");
        }

        AddEvent(new JobAddedEvent(Guid.NewGuid(), title, due, JobListInfo));
    }

    public void CompleteJob(Guid id)
    {
        Job? job = _jobs.FirstOrDefault(x => x.Id == id);

        if (job is null)
        {
            throw new ResourceNotFoundException<Job>(
                $"There is no job with the ID {id}");
        }

        if (job.IsComplete)
        {
            throw new DomainException<Job>($"The job with ID {id} was completed at {job.CompletedAt}");
        }

        AddEvent(new JobCompletedEvent(id, job.Title, JobListInfo));
    }

    private JobListInfo JobListInfo =>
        new(Id, Name, Category, Username);

    private void Apply(JobListCreatedEvent evt)
    {
        Id = evt.Id;
        Name = evt.Name;
        Category = evt.Category;
        Username = evt.Username;
    }

    private void Apply(JobAddedEvent evt) =>
        _jobs.Add(new Job(evt.Id, evt.Title, evt.Due));

    private void Apply(JobCompletedEvent evt) =>
        _jobs
            .First(x => x.Id == evt.Id)
            .Complete(evt.OccuredUtc);

    protected override void Apply(DomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case JobListCreatedEvent created:
                Apply(created);
                break;
            case JobAddedEvent jobAdded:
                Apply(jobAdded);
                break;
            case JobCompletedEvent jobCompleted:
                Apply(jobCompleted);
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(domainEvent),
                    $"There is no {nameof(Apply)} method for domain event of type {domainEvent.GetType().Name}");
        }
    }

    public static JobsList Replay(List<DomainEvent> domainEvents)
    {
        JobsList jobList = new();
        jobList.Apply(domainEvents);
        return jobList;
    }

    private JobsList()
    {

    }
}