// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosEventSourcing.Projections;
using Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Events;
using Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Items;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.CosmosEventSourcingAcceptanceTests.Projections;

public record TodoItemProjectionsKey : IProjectionKey;

public static class TodoItemProjections
{
    public class Added : IDomainEventProjection<TodoItemAdded, TodoListEventItem, TodoItemProjectionsKey>
    {
        private readonly IWriteOnlyRepository<TodoCosmosItem> _repository;
        private readonly ILogger<Added> _logger;

        public Added(
            IWriteOnlyRepository<TodoCosmosItem> repository,
            ILogger<Added> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async ValueTask HandleAsync(
            TodoItemAdded domainEvent,
            TodoListEventItem eventItem,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("TodoItemAdded being processed with ID {TodoId} and title {TodoTitle}",
                domainEvent.Id,
                domainEvent.Title);

            await _repository.UpdateAsync(new TodoCosmosItem(
                domainEvent.Id,
                domainEvent.Title,
                domainEvent.OccuredUtc,
                eventItem.PartitionKey), cancellationToken: cancellationToken);
        }
    }

    public class Completed : IDomainEventProjection<TodoItemCompleted, TodoListEventItem, TodoItemProjectionsKey>
    {
        private readonly IRepository<TodoCosmosItem> _repository;
        private readonly ILogger<Completed> _logger;

        public Completed(
            IRepository<TodoCosmosItem> repository,
            ILogger<Completed> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async ValueTask HandleAsync(
            TodoItemCompleted domainEvent,
            TodoListEventItem eventItem,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("TodoItemCompleted being processed with ID {TodoId} and title {TodoTitle}",
                domainEvent.Id,
                domainEvent.Title);

            TodoCosmosItem item = await _repository.GetAsync(
                domainEvent.Id.ToString(),
                eventItem.PartitionKey,
                cancellationToken);

            item.CompletedAt = domainEvent.OccuredUtc;

            await _repository.UpdateAsync(item, cancellationToken: cancellationToken);
        }
    }
}