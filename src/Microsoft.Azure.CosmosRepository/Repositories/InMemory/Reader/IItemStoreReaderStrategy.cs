// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.CosmosRepository.InMemory.Reader
{
    internal interface IItemStoreReaderStrategy<TItem> where TItem : IItem
    {
        ValueTask<TItem> TransformAsync(JObject itemJObject);

    }
}