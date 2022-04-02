// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.CosmosRepository.InMemory
{
    internal interface IItemStoreWriterStrategy<TItem> where TItem : IItem
    {
        ValueTask<string> TransformAsync(TItem item, string id, string? partitionKey = null);
    }

    internal sealed class ItemStoreWriterStrategy<TItem> : IItemStoreWriterStrategy<TItem> where TItem : IItem
    {
        private readonly IServiceProvider _serviceProvider;
        public ItemStoreWriterStrategy(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private IEnumerable<IItemStoreWriterStrategyStep> ResolveWriterStrategySteps()
        {
            Type itemType = typeof(TItem);
            Type[] itemTypes = itemType.GetInterfaces();
            List<IItemStoreWriterStrategyStep> services = new List<IItemStoreWriterStrategyStep>();

            foreach (Type type in itemTypes)
            {
                Type genericType = typeof(IItemStoreWriterStrategyStep<>).MakeGenericType(type);
                services.AddRange(_serviceProvider.GetServices(genericType)
                    .Select(service => (IItemStoreWriterStrategyStep)service));
            }

            return services;
        }

        public async ValueTask<string> TransformAsync(TItem item, string id, string? partitionKey = null)
        {
            IEnumerable<IItemStoreWriterStrategyStep> strategySteps = ResolveWriterStrategySteps();

            JObject itemJObject = JObject.FromObject(item);

            foreach (IItemStoreWriterStrategyStep strategyStep in strategySteps)
            {
                await strategyStep.TransformAsync(itemJObject, item);
            }

            return itemJObject.ToString(Formatting.Indented);
        }
    }
}