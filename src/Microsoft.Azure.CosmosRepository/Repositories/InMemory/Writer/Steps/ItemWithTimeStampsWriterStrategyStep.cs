// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository.Extensions;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.CosmosRepository.InMemory
{
    internal sealed class ItemWithTimeStampsWriterStrategyStep : ItemStoreWriterStrategyBase<IItemWithTimeStamps>
    {
        private const string TimeStampPropertyName = "_ts";
        public override ValueTask TransformAsync(
            JObject jObject,
            IItemWithTimeStamps item,
            CancellationToken cancellationToken = default)
        {
            jObject.AddOrUpdateProperty(TimeStampPropertyName, DateTimeOffset.Now.ToUnixTimeSeconds());

            item.CreatedTimeUtc ??= DateTime.UtcNow;

            return new ValueTask();
        }
    }
}
