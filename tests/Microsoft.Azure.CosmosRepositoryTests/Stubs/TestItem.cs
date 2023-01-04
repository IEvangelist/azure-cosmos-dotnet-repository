// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;

namespace Microsoft.Azure.CosmosRepositoryTests.Stubs;

public class TestItem : FullItem
{
    public TestItem() { }

    public TestItem(string etag) : base(etag) { }

    public string Property { get; set; } = default!;
}