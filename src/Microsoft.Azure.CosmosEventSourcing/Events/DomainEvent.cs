// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosEventSourcing.Events;

/// <summary>
/// A default implementation of a <see cref="IDomainEvent"/>
/// </summary>
public record DomainEvent : IDomainEvent
{
    /// <inheritdoc />
    public string EventId { get; init; } = Guid.NewGuid().ToString();

    ///<inheritdoc />
    public string EventName => GetType().Name;

    /// <inheritdoc />
    public int Sequence { get; init; }

    /// <inheritdoc />
    public DateTime OccuredUtc { get; init; }
};