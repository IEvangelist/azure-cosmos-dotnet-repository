// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Net.NetworkInformation;

namespace Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Entities;

public class TodoItem
{
    public int Id { get; }

    public string Title { get; }

    public DateTime? CompletedAt { get; private set; }

    public TodoItem(int id, string title)
    {
        Id = id;
        Title = title;
    }

    public void Completed(DateTime at) =>
        CompletedAt = at;
}