// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Azure.CosmosRepository;

namespace Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Items;

public class TodoCosmosItem : Item
{
    public string Title { get; set; }

    public DateTime CreatedUtc { get; set; }

    public string PartitionKey { get; set; }

    public DateTime? CompletedAt { get; set; }

    public bool IsComplete =>
        CompletedAt is not null;

    protected override string GetPartitionKeyValue() =>
        PartitionKey;

    public TodoCosmosItem(
        int id,
        string title,
        DateTime createdUtc,
        string listName)
    {
        Id = id.ToString();
        Title = title;
        CreatedUtc = createdUtc;
        PartitionKey = listName;
    }
}