// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository.InMemory.Reader;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.CosmosRepository.InMemory
{
    internal sealed class ItemStoreReaderStrategy<TItem> :
        IItemStoreReaderStrategy<TItem>
        where TItem : IItem
    {
        private readonly IJsonSerializer<TItem> _jsonSerializer;

        public ItemStoreReaderStrategy(
            IJsonSerializer<TItem> jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        public ValueTask<TItem> TransformAsync(
            JObject itemJObject,
            CancellationToken cancellationToken = default) =>
            new (_jsonSerializer.Deserialize(itemJObject));
    }
}