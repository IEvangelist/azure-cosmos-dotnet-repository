// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Aggregates;
using Microsoft.Azure.CosmosEventSourcing.Events;

namespace Microsoft.Azure.CosmosEventSourcing.Exceptions;

/// <summary>
/// An exception that is thrown when not events are provided to an <see cref="IAggregateRoot"/>.
/// </summary>
/// <remarks>An <see cref="IAggregateRoot"/> must be provided at least one <see cref="DomainEvent"/> when replaying events</remarks>
public class DomainEventsRequiredException : Exception
{
    /// <summary>
    /// Creates a <see cref="DomainEventsRequiredException"/>.
    /// </summary>
    public DomainEventsRequiredException(Type aggregateRootType) :
        base($"At least 1 {nameof(AtomicEvent)} must be provided for {aggregateRootType.Namespace} when replaying events")
    {
    }
}