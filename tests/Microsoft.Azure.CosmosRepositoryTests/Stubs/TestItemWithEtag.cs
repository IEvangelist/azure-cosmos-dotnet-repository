// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.Stubs;

public class TestItemWithEtag : Item, IItemWithEtag
{
    /// <summary>
    /// Etag for the item which was set by Cosmos the last time the item was updated. This string is used for the relevant operations when specified.
    /// </summary>
    [JsonProperty("_etag")]
    public string Etag { get; set; } = null!;
}