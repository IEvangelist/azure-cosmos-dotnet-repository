// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;

namespace EventSourcingJobsTracker.Infrastructure.Items;

public class JobsListReadItem : FullItem
{
    public string Name { get; }
    public string Username { get; }
    public string Category { get; }
    public string PartitionKey { get; set; }

    protected override string GetPartitionKeyValue() =>
        PartitionKey;

    public JobsListReadItem(
        string id,
        string name,
        string username,
        string category)
    {
        Id = id;
        Name = name;
        Username = username;
        Category = category;
        PartitionKey = username;
    }
}