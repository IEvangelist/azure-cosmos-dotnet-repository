// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Events;

namespace Microsoft.Azure.CosmosEventSourcing.Exceptions;

/// <summary>
/// An <see cref="Exception"/> that occurs when an <see cref="AtomicEvent"/> does not have it's Etag
/// </summary>
public class AtomicEventEtagRequiredException : Exception
{
    /// <summary>
    /// Creates an <see cref="AtomicEventRequiredException"/>
    /// </summary>
    public AtomicEventEtagRequiredException() : base($"An Etag must be provided when reading back an {nameof(AtomicEvent)}")
    {

    }
}