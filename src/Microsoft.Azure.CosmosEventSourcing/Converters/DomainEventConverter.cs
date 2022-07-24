// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.CosmosEventSourcing.Converters;

internal class DomainEventConverter : JsonConverter
{
    public static HashSet<Type> ConvertableTypes { get; } = new();

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
        serializer.Serialize(writer, value, value.GetType());

    public override object ReadJson(
        JsonReader reader,
        Type objectType,
        object existingValue,
        JsonSerializer serializer)
    {
        JToken? j = JToken.ReadFrom(reader);
        string? type = j["eventName"]?.ToString();
        type ??= j["EventName"]?.ToString();
        Type? payloadType = ConvertableTypes.FirstOrDefault(x => x.Name == type);

        if (payloadType is null)
        {
            return new NonDeserializableEvent
            {
                Name = type ?? "not-defined",
                Payload = JObject.Parse(reader.ReadAsString())
            };
        }

        return j.ToObject(payloadType);
    }

    public override bool CanConvert(Type objectType) =>
        typeof(IDomainEvent).IsAssignableFrom(objectType);
}