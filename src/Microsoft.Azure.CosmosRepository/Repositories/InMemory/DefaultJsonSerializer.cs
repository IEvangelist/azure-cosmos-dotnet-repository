// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.CosmosRepository.InMemory
{
    internal sealed class DefaultJsonSerializer<TItem> : IJsonSerializer<TItem> where TItem : IItem
    {
        public JObject Serialize(TItem item) => JObject.FromObject(item);

        public TItem Deserialize(JObject itemJObject) => itemJObject.ToObject<TItem>();
    }
}