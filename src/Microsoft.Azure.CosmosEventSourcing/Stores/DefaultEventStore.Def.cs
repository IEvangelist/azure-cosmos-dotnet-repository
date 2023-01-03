// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Runtime.CompilerServices.Context;
using Microsoft.Azure.CosmosEventSourcing.Items;
using Microsoft.Azure.CosmosRepository;

namespace Microsoft.Azure.CosmosEventSourcing.Stores;

internal partial class DefaultEventStore<TEventItem> :
    IEventStore<TEventItem> where TEventItem : EventItem
{
    private readonly IBatchRepository<TEventItem> _batchRepository;
    private readonly IReadOnlyRepository<TEventItem> _readOnlyRepository;
    private readonly IContextService _contextService;

    public DefaultEventStore(
        IBatchRepository<TEventItem> batchRepository,
        IReadOnlyRepository<TEventItem> readOnlyRepository,
        IContextService contextService)
    {
        _batchRepository = batchRepository;
        _readOnlyRepository = readOnlyRepository;
        _contextService = contextService;
    }
}