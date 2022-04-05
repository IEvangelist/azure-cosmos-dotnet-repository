// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
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
}