// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.CosmosRepository.InMemory
{
    internal interface IJsonSerializer<TItem> where TItem : IItem
    {
        JObject Serialize(TItem item);
        TItem Deserialize(JObject json);
    }
}