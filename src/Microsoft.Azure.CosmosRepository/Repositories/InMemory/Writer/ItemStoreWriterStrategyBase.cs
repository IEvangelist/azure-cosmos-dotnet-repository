// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.CosmosRepository.InMemory
{
    internal abstract class ItemStoreWriterStrategyBase<IItemType> : IItemStoreWriterStrategyStep<IItemType> where IItemType : IItem
    {
        public abstract ValueTask TransformAsync(JObject jObject, IItemType item);
        public ValueTask TransformAsync(JObject jObject, object item) => TransformAsync(jObject, (IItemType)item);
    }
}