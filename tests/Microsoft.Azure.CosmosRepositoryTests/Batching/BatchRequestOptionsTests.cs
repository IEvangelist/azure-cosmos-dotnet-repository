namespace Microsoft.Azure.CosmosRepositoryTests.Batching;

public class BatchRequestOptionsTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithMissingEtag_OmitsIfMatchEtag(string? etag)
    {
        TransactionalBatchItemRequestOptions options = BatchRequestOptions.Create(etag);

        options.IfMatchEtag.Should().BeNull();
    }

    [Fact]
    public void Create_WithEtag_SetsIfMatchEtag()
    {
        TransactionalBatchItemRequestOptions options = BatchRequestOptions.Create("etag");

        options.IfMatchEtag.Should().Be("etag");
    }
}
