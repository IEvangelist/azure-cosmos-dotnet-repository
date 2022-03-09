// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosEventSourcing.Events;

/// <summary>
/// A default implementation of a <see cref="IDomainEvent"/>
/// </summary>
public record DomainEvent : IDomainEvent
{
    ///<inheritdoc />
    public string EventName => GetType().Name;

    /// <inheritdoc />
    public int Sequence { get; set; }

    /// <inheritdoc />
    public DateTime OccuredUtc { get; set; }
};