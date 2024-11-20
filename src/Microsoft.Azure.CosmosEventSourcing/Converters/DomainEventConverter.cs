// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosEventSourcing.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.CosmosEventSourcing.Converters;

internal class DomainEventConverter : JsonConverter
{
    public static HashSet<Type> ConvertibleTypes { get; } = [];

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) =>
        serializer.Serialize(writer, value, objectType: value?.GetType() ?? throw new ArgumentNullException(nameof(value)));

    public override object? ReadJson(
        JsonReader reader,
        Type objectType,
        object? existingValue,
        JsonSerializer serializer)
    {
        try
        {
            var j = JToken.ReadFrom(reader);
            var type = j["eventName"]?.ToString();
            type ??= j["EventName"]?.ToString();
            Type? payloadType = ConvertibleTypes.FirstOrDefault(x => x.Name == type);

            return payloadType switch
            {
                null => new NonDeserializableEvent
                {
                    Name = type ?? "not-defined",
                    Payload = j.ToObject<JObject>() ?? []
                },
                _ => j.ToObject(payloadType)
            };
        }
        catch (Exception e)
        {
            return new NonDeserializableEvent
            {
                Exception = e,
                JsonReader = reader
            };
        }
    }

    public override bool CanConvert(Type objectType) =>
        typeof(IDomainEvent).IsAssignableFrom(objectType);
}