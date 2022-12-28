// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository;

/// <summary>
/// A base helper class that implements IItemWithTimeToLive
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
public abstract class TimeToLiveItem : Item, IItemWithTimeToLive
{
    /// <inheritdoc />
    public TimeSpan? TimeToLive
    {
        get => _timeToLive.HasValue ? TimeSpan.FromSeconds(_timeToLive.Value) : null;
        set => _timeToLive = (int?)value?.TotalSeconds;
    }

    [JsonProperty("ttl", NullValueHandling = NullValueHandling.Ignore)]
    private int? _timeToLive;
}