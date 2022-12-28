// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.CosmosEventSourcing.Events;

/// <summary>
/// This event is deserialized and presented in the place of an event that has not been configured for use by the custom JSON converter.
/// </summary>
public record NonDeserializableEvent : DomainEvent
{
    /// <summary>
    /// The eventName value that could not be deserialized.
    /// </summary>
    public string Name { get; init; } = "not-defined";

    /// <summary>
    /// The payload of the event whose type could not be found.
    /// </summary>
    public JObject Payload { get; init; } = JObject.FromObject(new { });

    /// <summary>
    /// An exception thrown as part of deserializing an event.
    /// </summary>
    public Exception? Exception { get; set; }

    /// <summary>
    /// The JSON reader that was used to try and deserialize the event.
    /// </summary>
    public JsonReader? JsonReader { get; set; }
}