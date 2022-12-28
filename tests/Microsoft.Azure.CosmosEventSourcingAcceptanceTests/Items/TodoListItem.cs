// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;

namespace Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Items;

public class TodoListItem : Item
{
    public string Name { get; set; }

    public string PartitionKey { get; set; }

    protected override string GetPartitionKeyValue() =>
        PartitionKey;

    public TodoListItem(string name)
    {
        Id = name;
        Name = name;
        PartitionKey = nameof(TodoListItem);
    }
}