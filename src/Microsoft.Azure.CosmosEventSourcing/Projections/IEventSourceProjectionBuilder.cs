// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosEventSourcing.Projections;

/// <summary>
/// Allows projections to built from an <see cref="EventSource"/>
/// </summary>
/// <typeparam name="TEventSource"></typeparam>
public interface IEventSourceProjectionBuilder<in TEventSource> where TEventSource : EventSource
{
    /// <summary>
    /// A method to process a new event after it has been saved into Cosmos.
    /// </summary>
    /// <remarks>This is invoked off the back fo the change feed processor library.</remarks>
    /// <param name="sourcedEvent">The event that was written.</param>
    /// <param name="cancellationToken">A token used to cancel the async operation.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the async operation</returns>
    ValueTask ProjectAsync(
        TEventSource sourcedEvent,
        CancellationToken cancellationToken = default);
}