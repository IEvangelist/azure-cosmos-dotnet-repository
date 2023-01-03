// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Events;

namespace Microsoft.Azure.CosmosEventSourcing.Exceptions;

/// <summary>
/// An event that is thrown when a <see cref="AtomicEvent"/> is not available where required.
/// </summary>
/// <remarks>This is required when replaying events.</remarks>
/// <remarks>This is required when persisting events.</remarks>
public class AtomicEventRequiredException : Exception
{
    /// <summary>
    /// Creates a <see cref="AtomicEventRequiredException"/>.
    /// </summary>
    public AtomicEventRequiredException(Type aggregateRootType) :
        base($"A {nameof(AtomicEvent)} must be provided for {aggregateRootType.Namespace} to perform this operation")
    {
    }

    /// <summary>
    /// Creates a <see cref="AtomicEventRequiredException"/>.
    /// </summary>
    public AtomicEventRequiredException() :
        base("A {nameof(AtomicEvent)} must be provided to perform this operation")
    {
    }
}