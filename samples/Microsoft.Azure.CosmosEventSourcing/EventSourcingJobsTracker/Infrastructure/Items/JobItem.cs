// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;

namespace EventSourcingJobsTracker.Infrastructure.Items;

public class JobItem : FullItem
{
    public string Title { get; }
    public DateTime Due { get; }
    public DateTime? CompletedAt { get; private set; }

    public string PartitionKey { get; set; }

    protected override string GetPartitionKeyValue() =>
        PartitionKey;

    public JobItem(
        string id,
        string jobListId,
        string title,
        DateTime due,
        DateTime? completedAt = null)
    {
        Id = id;
        Title = title;
        Due = due;
        CompletedAt = completedAt;
        PartitionKey = jobListId;
    }

    public void Complete(DateTime at) =>
        CompletedAt = at;
}