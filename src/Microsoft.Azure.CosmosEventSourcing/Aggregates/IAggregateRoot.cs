// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Events;

namespace Microsoft.Azure.CosmosEventSourcing.Aggregates;

/// <summary>
///
/// </summary>
public interface IAggregateRoot
{
    /// <summary>
    ///
    /// </summary>
    IReadOnlyList<DomainEvent> NewEvents { get; }

    /// <summary>
    ///
    /// </summary>
    IReadOnlyList<DomainEvent> Events { get; }
}