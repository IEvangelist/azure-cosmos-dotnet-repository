// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Microsoft.Azure.CosmosEventSourcing;

/// <summary>
/// A default implementation of a <see cref="IDomainEvent"/>
/// </summary>
public record DefaultDomainEvent : IDomainEvent
{
    ///<inheritdoc />
    public string EventName => GetType().Name;

    /// <inheritdoc />
    public int Sequence { get; set; }

    /// <inheritdoc />
    public DateTime OccuredUtc { get; set; }
};