// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository;

namespace Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Items;

public class TodoListItem : Item
{
    public string Name { get; set; }

    public string PartitonKey { get; set; }

    protected override string GetPartitionKeyValue() =>
        PartitonKey;

    public TodoListItem(string name)
    {
        Name = name;
        PartitonKey = nameof(TodoListItem);
    }
}