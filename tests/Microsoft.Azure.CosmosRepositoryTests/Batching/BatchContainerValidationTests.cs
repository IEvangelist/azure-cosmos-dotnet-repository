namespace Microsoft.Azure.CosmosRepositoryTests.Batching;

public class BatchContainerValidationTests
{
    [Fact]
    public void EnsureSameContainer_WhenResolverReturnsSharedName_DoesNotThrow()
    {
        Action act = () => BatchContainerValidation.EnsureSameContainer(
            [typeof(TestItem), typeof(TestItemOther)],
            _ => "shared");

        act.Should().NotThrow();
    }

    [Fact]
    public void EnsureSameContainer_WhenResolverReturnsDifferentNames_Throws()
    {
        Action act = () => BatchContainerValidation.EnsureSameContainer(
            [typeof(TestItem), typeof(TestItemOther)],
            itemType => itemType.Name);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage(BatchContainerValidation.MismatchMessage);
    }
}
