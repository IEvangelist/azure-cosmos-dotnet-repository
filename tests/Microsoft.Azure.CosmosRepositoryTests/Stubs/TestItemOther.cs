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
