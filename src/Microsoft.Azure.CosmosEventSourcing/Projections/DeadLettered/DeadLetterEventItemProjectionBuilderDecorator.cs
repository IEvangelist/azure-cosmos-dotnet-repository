// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Items;

namespace Microsoft.Azure.CosmosEventSourcing.Projections.DeadLettered;

// internal class DeadLetterProjectionBuilderDecorator<TEventItem> :
//     IEventItemProjectionBuilder<TEventItem>
//     where TEventItem : EventItem
// {
//     private readonly IEventItemProjectionBuilder<TEventItem> _innerHandler;
//
//     public DeadLetterProjectionBuilderDecorator(IEventItemProjectionBuilder<TEventItem> innerHandler)
//     {
//         _innerHandler = innerHandler;
//     }
//
//     public ValueTask ProjectAsync(TEventItem eventItem, CancellationToken cancellationToken = default)
//     {
//         throw new NotImplementedException();
//     }
// }