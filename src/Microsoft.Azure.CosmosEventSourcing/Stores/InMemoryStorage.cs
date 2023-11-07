// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Azure.CosmosEventSourcing.Items;
using Microsoft.Azure.CosmosRepository.ChangeFeed;
using Microsoft.Azure.CosmosRepository.ChangeFeed.Providers;
using Newtonsoft.Json;

namespace Microsoft.Azure.CosmosEventSourcing.Stores;

public static class InMemoryStorage
{
    private static readonly ConcurrentDictionary<string, string> Events = new();

    public static async ValueTask PersistAsync<TEventItem>(
        IEnumerable<TEventItem> events,
        string partitionKeyValue,
        IChangeFeedContainerProcessorProvider changeFeedContainerProcessorProvider)
        where TEventItem : EventItem
    {
        List<TEventItem> toSave;

        if (Events.TryGetValue(
                partitionKeyValue,
                out var currentJson))
        {
            toSave = JsonConvert.DeserializeObject<List<TEventItem>>(currentJson);
            toSave.AddRange(events);
        }
        else
        {
            toSave = events.ToList();
        }

        IEnumerable<IContainerChangeFeedProcessor> processors = changeFeedContainerProcessorProvider.GetProcessors();
        IEnumerable<IContainerChangeFeedProcessor> typesForEventItem = processors.Where(
            x =>
                x.GetType().GenericTypeArguments[0] == typeof(TEventItem));

        foreach (IContainerChangeFeedProcessor processor in typesForEventItem)
        {
            Type processorType = processor.GetType();
            MethodInfo? method = processorType.GetMethod("OnChangesAsync");
            var result = method?.Invoke(
                processor,
                new object[]
                {
                    events,
                    "in-memory",
                    CancellationToken.None
                });
            if (result is ValueTask valueTask)
            {
                await valueTask.AsTask();
            }
        }

        Events[partitionKeyValue] = JsonConvert.SerializeObject(toSave);
    }

    public static IEnumerable<TEventItem> ReadAllEvents<TEventItem>(
        string partitionKeyValue) where TEventItem : EventItem =>
        Events.TryGetValue(
            partitionKeyValue,
            out var currentJson)
            ? JsonConvert.DeserializeObject<List<TEventItem>>(currentJson)
            : new List<TEventItem>();
}