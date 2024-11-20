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

    private static readonly string[] s_sourceArray = ["🎶", "💿", "🎸", "🥁", "🎙"];

    public PagingTests() =>
        _options.Setup(monitor => monitor.CurrentValue)
            .Returns(_repositoryOptions);

    [Fact(Skip = "Unable to fake/mock ToFeedIterator.")]
    public async Task ReadOnlyRepositoriesDefaultImplementationOfPageAsyncYieldsCorrectly()
    {
        // Arrange
        TestItem[] items =
        [
            new TestItem { Number = 100, Property = "🎶 Record player" },
            new TestItem { Number = 101, Property = "💿 Vinyl albums" },
            new TestItem { Number = 102, Property = "🎸 Electric guitar" },
            new TestItem { Number = 103, Property = "🥁 Drums" },
            new TestItem { Number = 104, Property = "🎙 Microphone" },
            new TestItem { Number = 105, Property = "🎚 Channels" },
            new TestItem { Number = 106, Property = "🎛 Mixer" },
            new TestItem { Number = 107, Property = "🎧 Headphones" },
            new TestItem { Number = 108, Property = "🎹 Keys" },
            new TestItem { Number = 109, Property = "🎷 Saxophone" },
            new TestItem { Number = 110, Property = "🎺 Trumpet" },
            new TestItem { Number = 111, Property = "🎵 Music" },
            new TestItem { Number = 112, Property = "🎨 Art" },
            new TestItem { Number = 113, Property = "🎭 Self-expression" }
        ];

        _containerProviderForTestItem.Setup(
            cp => cp.GetContainerAsync()).ReturnsAsync(_container.Object);

        Expression<Func<TestItem, bool>> predicate =
            static (TestItem item) => item.Number > 108;

        IOrderedQueryable<TestItem> queryable = items.AsQueryable().OrderBy(
            item => (int)item.Property[0]);

        _container
            .Setup(o => o.GetItemLinqQueryable<TestItem>(false, null, null, null))
            .Returns(queryable);

        _queryableProcessor.Setup(o => o.IterateAsync(queryable, It.IsAny<CancellationToken>()))
            .ReturnsAsync((items, 0));

        IReadOnlyRepository<TestItem> repository = RepositoryForTestItem;

        await Task.CompletedTask;

        // TODO: Test this functionality.

        // Act
        //await foreach (TestItem actualItem in repository.PageAsync(
        //    predicate,
        //    limit: 5, // The first five test item.
        //    CancellationToken.None))
        //{
        //    // Assert
        //    Assert.Contains(actualItem, items);
        //}
    }
}
#endif