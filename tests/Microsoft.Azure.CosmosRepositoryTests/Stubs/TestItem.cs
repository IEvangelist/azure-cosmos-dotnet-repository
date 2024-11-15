// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.Stubs;

public class TestItem : FullItem
{
    public TestItem() { }

    public TestItem(string etag) : base(etag) { }

    public string Property { get; set; } = default!;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int Number { get; set; }

    public IEnumerable<string> Items { get; set; } = default!;
}