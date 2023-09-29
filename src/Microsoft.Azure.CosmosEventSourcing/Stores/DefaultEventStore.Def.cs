// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Runtime.CompilerServices.Context;
using Microsoft.Azure.CosmosEventSourcing.Items;
using Microsoft.Azure.CosmosRepository;

namespace Microsoft.Azure.CosmosEventSourcing.Stores;

internal partial class DefaultEventStore<TEventItem>(
    IBatchRepository<TEventItem> batchRepository,
    IReadOnlyRepository<TEventItem> readOnlyRepository,
    IContextService contextService) :
    IEventStore<TEventItem> where TEventItem : EventItem
{
}