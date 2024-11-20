// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.


// ReSharper disable once CheckNamespace
namespace Microsoft.Azure.CosmosRepository;

internal partial class InMemoryRepository<TItem>
{
    private static string SerializeItem(
        TItem item,
        string? etag = null,
        long? ts = null)
    {
        var jObject = JObject.FromObject(item);
        if (etag != null)
        {
            jObject["_etag"] = JToken.FromObject(etag);
        }

        if (ts.HasValue)
        {
            jObject["_ts"] = JToken.FromObject(ts);
        }

        return jObject.ToString();
    }

    internal TItem DeserializeItem(string jsonItem) =>
        JsonConvert.DeserializeObject<TItem>(jsonItem)!;

    internal static TDeserializeTo DeserializeItem<TDeserializeTo>(string jsonItem) =>
        JsonConvert.DeserializeObject<TDeserializeTo>(jsonItem)!;
}