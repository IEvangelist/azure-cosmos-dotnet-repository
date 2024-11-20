// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.ChangeFeed;

internal class DefaultContainerChangeFeedProcessor : IContainerChangeFeedProcessor
{
    private readonly ICosmosContainerService _containerService;
    private readonly ILeaseContainerProvider _leaseContainerProvider;
    private readonly ChangeFeedOptions _changeFeedOptions;
    private readonly ILogger<DefaultContainerChangeFeedProcessor> _logger;
    private readonly IServiceProvider _serviceProvider;
    private ChangeFeedProcessor? _processor;
    private static readonly ConcurrentDictionary<Type, Type> Handlers = new();

    public DefaultContainerChangeFeedProcessor(
        ICosmosContainerService containerService,
        IReadOnlyList<Type> itemTypes,
        ILeaseContainerProvider leaseContainerProvider,
        ChangeFeedOptions changeFeedOptions,
        ILogger<DefaultContainerChangeFeedProcessor> logger,
        IServiceProvider serviceProvider)
    {
        itemTypes.AreAllItems();
        _containerService = containerService;
        ItemTypes = itemTypes;
        _leaseContainerProvider = leaseContainerProvider;
        _changeFeedOptions = changeFeedOptions;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public IReadOnlyList<Type> ItemTypes { get; }

    public async Task StartAsync()
    {
        Container itemContainer = await _containerService.GetContainerAsync(ItemTypes);
        Container leaseContainer = await _leaseContainerProvider.GetLeaseContainerAsync();

        ChangeFeedProcessorBuilder builder = itemContainer
            .GetChangeFeedProcessorBuilder<JObject>(_changeFeedOptions.ProcessorName,
                (changes, token) => OnChangesAsync(changes, itemContainer.Id, token))
            .WithLeaseContainer(leaseContainer)
            .WithInstanceName(_changeFeedOptions.InstanceName)
            .WithErrorNotification((_, exception) => OnErrorAsync(exception, itemContainer.Id));

        if (_changeFeedOptions.PollInterval != null)
        {
            builder.WithPollInterval(_changeFeedOptions.PollInterval.Value);
        }

        if (_changeFeedOptions.StartTime.HasValue)
        {
            builder.WithStartTime(_changeFeedOptions.StartTime.Value);
        }
        _processor = builder.Build();

        _logger.LogInformation("Starting change feed processor for container {ContainerName}", itemContainer.Id);

        await _processor.StartAsync();

        _logger.LogInformation("Successfully started change feed processor for container {ContainerName}",
            itemContainer.Id);
    }

    internal async Task OnChangesAsync(
        IReadOnlyCollection<JObject> changes,
        string containerName,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Detected changes for container {ContainerName} total ({ChangesCount})",
            containerName, changes.Count);

        foreach (JObject change in changes)
        {
            if (!change.TryGetValue("type", out JToken? type))
            {
                //TODO: _logger.LogWarning(...) here? Should this be turn on/off thing? A user might not have created documents with the type field ?!
                continue;
            }

            Type? itemType = ItemTypes.FirstOrDefault(x => x.Name == type.Value<string>());

            if (itemType is null)
            {
                _logger.LogDebug(
                    "No change feed processor registered for type {ItemType} in container {ContainerName}",
                    type.Value<string>(), containerName);

                continue;
            }

            await InvokeHandlerAsync(itemType, change, cancellationToken);
        }
    }

    private async Task InvokeHandlerAsync(
        Type itemType,
        JObject instance,
        CancellationToken cancellationToken)
    {
        var item = instance.ToObject(itemType);

        Type? handlerType = null;

        if (Handlers.ContainsKey(itemType) is false)
        {
            handlerType = typeof(IItemChangeFeedProcessor<>).MakeGenericType(itemType);
            Handlers[itemType] = handlerType;
        }

        handlerType ??= Handlers[itemType];

        IEnumerable<object?> handlers = _serviceProvider.GetServices(handlerType).ToList();

        _logger.LogDebug("Invoking IItemChangeFeedProcessor's ({ProcessorsCount}) for item type {ItemType}",
            handlers.Count(), itemType);

        await Task.WhenAll(handlers.Select(handler =>
        {
            try
            {
                var response = handlerType.GetMethod("HandleAsync")?
                    .Invoke(handler, [item, cancellationToken]);

                if (response is ValueTask valueTask)
                {
                    return valueTask.AsTask();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    "Failed to handle change for item of type {ItemType} when invoking IItemChangeFeedProcessor {ProcessorTypeName}",
                    itemType, handler?.GetType().Name);
            }

            return Task.CompletedTask;
        }));
    }

    private Task OnErrorAsync(Exception exception, string containerName)
    {
        _logger.LogError(exception, "Failed handling when handling changes detected from container {ContainerName}",
            containerName);
        return Task.CompletedTask;
    }

    public Task StopAsync() =>
        _processor?.StopAsync() ?? Task.CompletedTask;
}