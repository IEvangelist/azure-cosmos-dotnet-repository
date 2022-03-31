// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository.Extensions;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.CosmosRepository.InMemory
{
    internal interface IItemStoreWriterStrategyStep
    {
        ValueTask TransformAsync(JObject jObject, Object item);
    }

    internal interface IItemStoreWriterStrategyStep<IItemType> : IItemStoreWriterStrategyStep where IItemType : IItem
    {
        ValueTask TransformAsync(JObject jObject, IItemType item);
    }

    internal abstract class ItemStoreWriterStrategyBase<IItemType> : IItemStoreWriterStrategyStep<IItemType> where IItemType : IItem
    {
        public abstract ValueTask TransformAsync(JObject jObject, IItemType item);
        public ValueTask TransformAsync(JObject jObject, object item) => TransformAsync(jObject, (IItemType)item);
    }

    internal sealed class ItemStoreWithTimeStampsWriterStrategyStep : ItemStoreWriterStrategyBase<IItemWithTimeStamps>
    {
        private const string TimeStampPropertyName = "_ts";
        public override ValueTask TransformAsync(JObject jObject, IItemWithTimeStamps item)
        {
            jObject.AddOrUpdateProperty(TimeStampPropertyName, DateTimeOffset.Now.ToUnixTimeSeconds());
            return new ValueTask();
        }
    }
}