// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository;

/// <summary>
/// A base helper class that implements IItemWithEtag
/// </summary>
/// <example>
/// Here is an example subclass item, which adds several properties:
/// <code language="c#">
/// <![CDATA[
/// public class SubItem : EtagItem
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
public abstract class EtagItem : Item, IItemWithEtag
{
    /// <summary>
    /// Default constructor
    /// </summary>
    protected EtagItem()
    {
    }

    /// <summary>
    /// A constructor that allows the etag to be set so that items can be mapped to and from other objects
    /// </summary>
    /// <param name="etag"></param>
    protected EtagItem(string etag)
    {
        Etag = etag;
    }

    /// <summary>
    /// Etag for the item which was set by Cosmos the last time the item was updated. This string is used for the relevant operations when specified.
    /// </summary>
    [JsonProperty("_etag")]
    public string Etag { get; private set; } = null!;
}
