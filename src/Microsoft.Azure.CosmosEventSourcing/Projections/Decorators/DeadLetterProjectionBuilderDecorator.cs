// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Items;
using Microsoft.Azure.CosmosEventSourcing.Options;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.CosmosEventSourcing.Projections.Decorators;

internal class DeadLetterProjectionBuilderDecorator<TEventItem, TProjectionKey> :
    IEventItemProjectionBuilder<TEventItem, TProjectionKey>
    where TEventItem : EventItem
    where TProjectionKey : IProjectionKey

{
    private readonly ILogger<DeadLetterProjectionBuilderDecorator<TEventItem, TProjectionKey>> _logger;
    //TODO: should this include the projection key?
    private readonly EventSourcingProcessorOptions<TEventItem> _processorOptions;
    private readonly IEventItemProjectionBuilder<TEventItem, TProjectionKey> _innerBuilder;
    private readonly DeadLetterOptions<TEventItem, TProjectionKey> _options;
    private readonly IWriteOnlyRepository<DeadLetteredEventItem<TEventItem>> _repository;

    public DeadLetterProjectionBuilderDecorator(
        ILogger<DeadLetterProjectionBuilderDecorator<TEventItem, TProjectionKey>> logger,
        EventSourcingProcessorOptions<TEventItem> processorOptions,
        IEventItemProjectionBuilder<TEventItem, TProjectionKey> innerBuilder,
        DeadLetterOptions<TEventItem, TProjectionKey> options,
        IWriteOnlyRepository<DeadLetteredEventItem<TEventItem>> repository)
    {
        _logger = logger;
        _processorOptions = processorOptions;
        _innerBuilder = innerBuilder;
        _options = options;
        _repository = repository;
    }

    public async ValueTask ProjectAsync(TEventItem eventItem, CancellationToken cancellationToken = default)
    {
        try
        {
            await _innerBuilder.ProjectAsync(eventItem, cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}