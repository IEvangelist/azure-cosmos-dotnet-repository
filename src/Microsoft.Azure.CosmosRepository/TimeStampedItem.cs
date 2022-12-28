// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository;

/// <summary>
/// A base helper class that implements IItemWithTimeStamps
/// </summary>
/// <example>
/// Here is an example subclass item, which adds several properties:
/// <code language="c#">
/// <![CDATA[
/// public class SubItem : ItemWithTimeToLive
/// {
///     public DateTimeOffset Date { get; set; }
///     public string Name { get; set; }
///     public IEnumerable<Child> Children { get; set; }
///     public IEnumerable<string> Tags { get; set; }
/// }
///
/// public class Child
/// {
///     public string Name { get; set; }
///     public DateTime BirthDate { get; set; }
/// }
/// ]]>
/// </code>
/// </example>
public class TimeStampedItem : Item, IItemWithTimeStamps
{
    /// <inheritdoc />
    [JsonProperty]
    public DateTime? CreatedTimeUtc { get; set; }

    /// <inheritdoc />
    [JsonIgnore]
    public DateTime LastUpdatedTimeUtc => DateTimeOffset.FromUnixTimeSeconds(LastUpdatedTimeRaw).DateTime;

    /// <inheritdoc />
    [JsonProperty("_ts")]
    public long LastUpdatedTimeRaw { get; }
}