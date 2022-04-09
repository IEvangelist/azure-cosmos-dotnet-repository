// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository.InMemory.Reader;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.CosmosRepository.InMemory
{
    internal sealed class ItemStoreWriterStrategy<TItem> :
        IItemStoreWriterStrategy<TItem>
        where TItem : IItem
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IJsonSerializer<TItem> _jsonSerializer;

        public ItemStoreWriterStrategy(IServiceProvider serviceProvider, IJsonSerializer<TItem> jsonSerializer)
        {
            _serviceProvider = serviceProvider;
            _jsonSerializer = jsonSerializer;
        }

        private IEnumerable<IItemStoreWriterStrategyStep> ResolveWriterStrategySteps()
        {
            Type itemType = typeof(TItem);
            Type[] itemTypes = itemType.GetInterfaces();
            List<IItemStoreWriterStrategyStep> services = new();

            foreach (Type type in itemTypes)
            {
                Type genericType = typeof(IItemStoreWriterStrategyStep<>).MakeGenericType(type);
                services.AddRange(_serviceProvider.GetServices(genericType)
                    .Select(service => (IItemStoreWriterStrategyStep)service));
            }

            return services;
        }

        public async ValueTask<JObject> TransformAsync(
            TItem item,
            string id,
            string? partitionKey = null,
            CancellationToken cancellationToken = default)
        {
            IEnumerable<IItemStoreWriterStrategyStep> strategySteps = ResolveWriterStrategySteps();

            JObject itemJObject = _jsonSerializer.Serialize(item);

            foreach (IItemStoreWriterStrategyStep strategyStep in strategySteps)
            {
                await strategyStep.TransformAsync(itemJObject, item, cancellationToken);
            }

            return itemJObject;
        }
    }
}