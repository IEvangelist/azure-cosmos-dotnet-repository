// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.CosmosRepository.InMemory
{
    internal abstract class ItemStoreWriterStrategyBase<IItemType> : IItemStoreWriterStrategyStep<IItemType> where IItemType : IItem
    {
        public abstract ValueTask TransformAsync(
            JObject jObject,
            IItemType item,
            CancellationToken cancellationToken = default);

        public ValueTask TransformAsync(
            JObject jObject,
            object item,
            CancellationToken cancellationToken = default) =>
            TransformAsync(jObject, (IItemType)item, cancellationToken);
    }
}