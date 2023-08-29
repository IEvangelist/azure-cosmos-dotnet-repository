// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.Paging;

#if NET7_0_OR_GREATER
public partial class PagingTests
{
    readonly Mock<ICosmosContainerProvider<TestItem>> _containerProviderForTestItem = new();
    readonly Mock<ICosmosQueryableProcessor> _queryableProcessor = new();
    readonly Mock<IOptionsMonitor<RepositoryOptions>> _options = new();
    readonly RepositoryOptions _repositoryOptions = new();
    readonly Mock<Container> _container = new();
    readonly IRepositoryExpressionProvider _expressionProvider = new MockExpressionProvider();
    readonly ISpecificationEvaluator _specificationEvaluator = new SpecificationEvaluator();

    private DefaultRepository<TestItem> RepositoryForTestItem =>
        new(_options.Object,
            _containerProviderForTestItem.Object,
            new NullLogger<DefaultRepository<TestItem>>(),
            _queryableProcessor.Object,
            _expressionProvider,
            _specificationEvaluator);

    public PagingTests() =>
        _options.Setup(monitor => monitor.CurrentValue)
            .Returns(_repositoryOptions);

    [Fact]
    public async Task ReadOnlyRepositoriesDefaultImplementationOfPageAsyncYieldsCorrectly()
    {
        // Arrange
        TestItem[] items = new[]
        {
            new TestItem { Property = "🎶 Record player" },
            new TestItem { Property = "💿 Vinyl albums" },
            new TestItem { Property = "🎸 Electric guitar" },
            new TestItem { Property = "🥁 Drums" },
            new TestItem { Property = "🎙 Microphone" },
            new TestItem { Property = "🎚 Channels" },
            new TestItem { Property = "🎛 Mixer" },
            new TestItem { Property = "🎧 Headphones" },
            new TestItem { Property = "🎹 Keys" },
            new TestItem { Property = "🎷 Saxophone" },
            new TestItem { Property = "🎺 Trumpet" },
            new TestItem { Property = "🎵 Music" },
            new TestItem { Property = "🎨 Art" },
            new TestItem { Property = "🎭 Self-expression" }
        };

        _containerProviderForTestItem.Setup(
            cp => cp.GetContainerAsync()).ReturnsAsync(_container.Object);

        Expression<Func<TestItem, bool>> predicate =
            static (TestItem item) =>
                item.Property.StartsWith("🎶") ||
                item.Property.StartsWith("💿") ||
                item.Property.StartsWith("🎸") ||
                item.Property.StartsWith("🥁") ||
                item.Property.StartsWith("🎙");

        IOrderedQueryable<TestItem> queryable = items.AsQueryable().OrderBy(
            item => (int)item.Property[0]);

        _container
            .Setup(o => o.GetItemLinqQueryable<TestItem>(false, null, null, null))
            .Returns(queryable);

        _queryableProcessor.Setup(o => o.IterateAsync(queryable, It.IsAny<CancellationToken>()))
            .ReturnsAsync((items, 0));

        IReadOnlyRepository<TestItem> repository = RepositoryForTestItem;

        await ValueTask.CompletedTask;

        // TODO: Test this functionality.

        // Act
        await foreach (TestItem actualItem in repository.PageAsync(
            predicate,
            limit: 5, // The first five test item.
            CancellationToken.None))
        {
            // Assert
            Assert.Contains(actualItem, items);
        }
    }
}
#endif