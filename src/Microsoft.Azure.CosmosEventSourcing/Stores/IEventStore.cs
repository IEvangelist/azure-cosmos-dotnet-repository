// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Items;

namespace Microsoft.Azure.CosmosEventSourcing.Stores;

/// <summary>
/// The interface responsible for managing the persistence of the an <see cref="EventItem"/>
/// </summary>
/// <typeparam name="TEventItem"></typeparam>
public interface IEventStore<TEventItem> :
    IReadOnlyEventStore<TEventItem>,
    IWriteOnlyEventStore<TEventItem>
    where TEventItem : EventItem
{
}