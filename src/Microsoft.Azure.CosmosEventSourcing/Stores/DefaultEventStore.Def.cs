// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Runtime.CompilerServices.Context;
using Microsoft.Azure.CosmosEventSourcing.Items;
using Microsoft.Azure.CosmosEventSourcing.Options;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.CosmosEventSourcing.Stores;

internal partial class DefaultEventStore<TEventItem>(
    IBatchRepository<TEventItem> batchRepository,
    IReadOnlyRepository<TEventItem> readOnlyRepository,
    IContextService contextService,
    IOptionsMonitor<CosmosEventSourcingOptions> optionsMonitor) :
    IEventStore<TEventItem> where TEventItem : EventItem
{
    private readonly IOptionsMonitor<CosmosEventSourcingOptions> _optionsMonitor = optionsMonitor;
}