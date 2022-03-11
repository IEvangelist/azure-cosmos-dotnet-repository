// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Microsoft.Azure.CosmosEventSourcing.Events;

/// <summary>
/// A default implementation of a <see cref="IDomainEvent"/>
/// </summary>
public record DomainEvent: IDomainEvent
{
    ///<inheritdoc />
    public string EventName => GetType().Name;

    /// <inheritdoc />
    [JsonProperty("sequence")]
    public int Sequence { get; init; }

    /// <inheritdoc />
    public DateTime OccuredUtc { get; init; }
};