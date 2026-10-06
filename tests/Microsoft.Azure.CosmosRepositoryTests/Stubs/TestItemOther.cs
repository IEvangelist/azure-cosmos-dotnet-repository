// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.Stubs;

public class TestItemOther : FullItem
{
    public TestItemOther()
    {
    }

    public TestItemOther(string etag) : base(etag)
    {
    }

    public string Property { get; set; } = default!;
}
