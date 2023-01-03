// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace EventSourcingJobsTracker.Core.Entities;

public class Job
{
    public Guid Id { get; }
    public string Title { get; }
    public DateTime Due { get; }

    public DateTime? CompletedAt { get; private set; }

    public bool IsComplete =>
        CompletedAt is not null;

    public Job(Guid id, string title, DateTime due)
    {
        Id = id;
        Title = title;
        Due = due;
    }

    public void Complete(DateTime at) => CompletedAt = at;
}