// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System;

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