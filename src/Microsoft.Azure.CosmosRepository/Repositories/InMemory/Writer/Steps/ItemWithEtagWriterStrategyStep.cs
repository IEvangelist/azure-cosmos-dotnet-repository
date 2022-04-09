// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository.Extensions;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.CosmosRepository.InMemory
{
    internal sealed class ItemWithEtagWriterStrategyStep : ItemStoreWriterStrategyBase<IItemWithEtag>
    {
        private const string EtagPropertyName = "_etag";
        public override ValueTask TransformAsync(
            JObject jObject,
            IItemWithEtag item,
            CancellationToken cancellationToken = default)
        {
            jObject.AddOrUpdateProperty(EtagPropertyName, Guid.NewGuid().ToString());
            return new ValueTask();
        }
    }
}