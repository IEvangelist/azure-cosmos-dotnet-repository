// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Events;

namespace Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Events;

public record TodoListCreated(
    string Name) : DomainEvent;