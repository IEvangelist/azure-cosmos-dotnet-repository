// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Extensions;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Processors;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Azure.CosmosRepository.Specification.Evaluator;
using Microsoft.Azure.CosmosRepositoryTests.Stubs;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Microsoft.Azure.CosmosRepositoryTests
{
    public class DefaultRepositoryTests
    {
        readonly Mock<ICosmosContainerProvider<TestItemWithEtag>> _containerProviderForTestItemWithETag = new();
        readonly Mock<ICosmosContainerProvider<TestItem>> _containerProviderForTestItem = new();
        readonly Mock<ICosmosQueryableProcessor> _queryableProcessor = new();
        readonly Mock<IOptionsMonitor<RepositoryOptions>> _options = new();
        readonly RepositoryOptions _repositoryOptions = new();
        readonly Mock<Container> _container = new();
        readonly IRepositoryExpressionProvider _expressionProvider = new MockExpressionProvider();
        readonly ISpecificationEvaluator _specificationEvaluator = new SpecificationEvaluator();
        public DefaultRepositoryTests()
        {
            _options.Setup(o => o.CurrentValue).Returns(_repositoryOptions);
        }

        private DefaultRepository<TestItemWithEtag> RepositoryForItemWithETag =>
            new(_options.Object,
                _containerProviderForTestItemWithETag.Object,
                new NullLogger<DefaultRepository<TestItemWithEtag>>(),
                _queryableProcessor.Object,
                _expressionProvider,
                _specificationEvaluator);

        private DefaultRepository<TestItem> RepositoryForItemWithoutETag =>
            new(_options.Object,
                _containerProviderForTestItem.Object,
                new NullLogger<DefaultRepository<TestItem>>(),
                _queryableProcessor.Object,
                _expressionProvider,
                _specificationEvaluator);

        [Fact]
        public async Task GetAsyncGivenExpressionQueriesContainerCorrectly()
        {
            //Arrange
            List<TestItemWithEtag> items = new()
            {
                new() { Id = "a" },
                new() { Id = "c" },
                new() { Id = "ab" }
            };

            Expression<Func<TestItemWithEtag, bool>> predicate = item => item.Id == "a" || item.Id == "ab";

            Expression<Func<TestItemWithEtag, bool>> expectedPredicate = _expressionProvider.Build(predicate)!;

            IOrderedQueryable<TestItemWithEtag> queryable = items.AsQueryable().Where(expectedPredicate).OrderBy(i => i.Id);
            _containerProviderForTestItemWithETag.Setup(o => o.GetContainerAsync()).ReturnsAsync(_container.Object);

            _container
                .Setup(o => o.GetItemLinqQueryable<TestItemWithEtag>(false, null, null, null))
                .Returns(queryable);

            _queryableProcessor.Setup(o => o.IterateAsync(queryable, It.IsAny<CancellationToken>()))
                .ReturnsAsync((items, 0));

            //Act
            IEnumerable<TestItemWithEtag> result = await RepositoryForItemWithETag.GetAsync(predicate);

            //Assert
            Assert.Equal(items, result);
            Assert.Equal(2, queryable.ToList().Count);
        }

        [Fact]
        public async Task ExistsAsyncGivenExpressionQueriesContainerCorrectly()
        {
            //Arrange
            List<TestItemWithEtag> items = new()
            {
                new(){Id = "a"},
                new(){Id = "c"},
                new(){Id = "ab"}
            };

            Expression<Func<TestItemWithEtag, bool>> predicate = item => item.Id == "a" || item.Id == "ab";

            Expression<Func<TestItemWithEtag, bool>> expectedPredicate = _expressionProvider.Build(predicate)!;

            IOrderedQueryable<TestItemWithEtag> queryable = items.AsQueryable().Where(expectedPredicate).OrderBy(i => i.Id);
            _containerProviderForTestItemWithETag.Setup(o => o.GetContainerAsync()).ReturnsAsync(_container.Object);

            _container
                .Setup(o => o.GetItemLinqQueryable<TestItemWithEtag>(false, null, null, null))
                .Returns(queryable);

            _queryableProcessor.Setup(o => o.CountAsync(queryable, It.IsAny<CancellationToken>()))
                .ReturnsAsync(items.Count);

            //Act
            bool result = await RepositoryForItemWithETag.ExistsAsync(predicate);

            //Assert
            Assert.True(result);
            Assert.Equal(2, queryable.ToList().Count);
        }

        [Fact]
        public async Task CreateAsync_SingleItemWithTimeStamps_WhenCreatedTimeStampHasNotBeenSet_ShouldSetCreatedTimeStamp()
        {
            // Arrange
            TestItem item = new();

            _containerProviderForTestItem.Setup(cp => cp.GetContainerAsync()).ReturnsAsync(_container.Object);
            _container.Setup(x => x.CreateItemAsync(It.IsAny<TestItem>(), It.IsAny<PartitionKey>(), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Mock<ItemResponse<TestItem>>().Object);

            // Act
            await RepositoryForItemWithoutETag.CreateAsync(item);

            // Assert
            Assert.NotNull(item.CreatedTimeUtc);
            Assert.InRange(item.CreatedTimeUtc!.Value, DateTime.UtcNow.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));
        }

        [Fact]
        public async Task CreateAsync_SingleItemWithTimeStamps_WhenCreatedTimeStampHasAlreadyBeenSet_ShouldNotSetCreatedTimeStamp()
        {
            // Arrange
            DateTime expectedCreatedTimeUtc = DateTime.UtcNow.AddDays(-7);
            TestItem item = new()
            {
                CreatedTimeUtc = expectedCreatedTimeUtc
            };

            _containerProviderForTestItem.Setup(cp => cp.GetContainerAsync()).ReturnsAsync(_container.Object);
            _container.Setup(x => x.CreateItemAsync(It.IsAny<TestItem>(), It.IsAny<PartitionKey>(), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Mock<ItemResponse<TestItem>>().Object);

            // Act
            await RepositoryForItemWithoutETag.CreateAsync(item);

            // Assert
            Assert.NotNull(item.CreatedTimeUtc);
            Assert.Equal(expectedCreatedTimeUtc, item.CreatedTimeUtc!.Value);
        }

        [Fact]
        public async Task UpdateAsync_SingleItem_WhenIgnoreEtagIsFalseAndEtagIsProvided_UseEtagInUpsertOptions()
        {
            // Arrange
            TestItemWithEtag itemWithEtag = new()
            {
                Etag = Guid.NewGuid().ToString()
            };

            _containerProviderForTestItemWithETag.Setup(cp => cp.GetContainerAsync()).ReturnsAsync(_container.Object);

            // Act
            await RepositoryForItemWithETag.UpdateAsync(itemWithEtag);

            // Assert
            _container.Verify(container => container.UpsertItemAsync(itemWithEtag, new PartitionKey(itemWithEtag.Id), It.Is<ItemRequestOptions>(options => options.IfMatchEtag == itemWithEtag.Etag), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_SingleItem_WhenIgnoreEtagIsFalseAndItemTypeIsNotAssignableToIItemWithEtag_UseDefaultEtagValueInUpsertOptions()
        {
            // Arrange
            TestItem item = new();

            _containerProviderForTestItem.Setup(cp => cp.GetContainerAsync()).ReturnsAsync(_container.Object);

            // Act
            await RepositoryForItemWithoutETag.UpdateAsync(item);

            // Assert
            _container.Verify(
                container => container.UpsertItemAsync(item, new PartitionKey(item.Id),
                    It.Is<ItemRequestOptions>(options => options.IfMatchEtag == default),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_SingleItem_WhenIgnoreEtagIsFalseAndEtagIsNull_UseDefaultEtagValueInUpsertOptions()
        {
            // Arrange
            TestItemWithEtag itemWithEtag = new()
            {
                Etag = null!
            };

            _containerProviderForTestItemWithETag.Setup(cp => cp.GetContainerAsync()).ReturnsAsync(_container.Object);

            // Act
            await RepositoryForItemWithETag.UpdateAsync(itemWithEtag);

            // Assert
            _container.Verify(
                container => container.UpsertItemAsync(itemWithEtag, new PartitionKey(itemWithEtag.Id),
                    It.Is<ItemRequestOptions>(options => options.IfMatchEtag == default),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_SingleItem_WhenIgnoreEtagIsTrueAndEtagIsNull_UseDefaultEtagValueInUpsertOptions()
        {
            // Arrange
            TestItemWithEtag itemWithEtag = new()
            {
                Etag = null!
            };

            _containerProviderForTestItemWithETag.Setup(cp => cp.GetContainerAsync()).ReturnsAsync(_container.Object);

            // Act
            await RepositoryForItemWithETag.UpdateAsync(itemWithEtag, default, true);

            // Assert
            _container.Verify(
                container => container.UpsertItemAsync(itemWithEtag, new PartitionKey(itemWithEtag.Id),
                    It.Is<ItemRequestOptions>(options => options.IfMatchEtag == default),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        //////////
        [Fact]
        public async Task UpdateAsync_MultipleItems_WhenIgnoreEtagIsFalseAndEtagIsProvided_UseEtagInUpsertOptions()
        {
            // Arrange
            TestItemWithEtag itemWithEtag1 = new()
            {
                Etag = Guid.NewGuid().ToString()
            };

            TestItemWithEtag itemWithEtag2 = new()
            {
                Etag = Guid.NewGuid().ToString()
            };

            IEnumerable<TestItemWithEtag> testItemWithEtags = new []
            {
                itemWithEtag1,
                itemWithEtag2
            };

            _containerProviderForTestItemWithETag.Setup(cp => cp.GetContainerAsync()).ReturnsAsync(_container.Object);

            // Act
            await RepositoryForItemWithETag.UpdateAsync(testItemWithEtags);

            // Assert
            _container.Verify(container => container.UpsertItemAsync(itemWithEtag1, new PartitionKey(itemWithEtag1.Id), It.Is<ItemRequestOptions>(options => options.IfMatchEtag == itemWithEtag1.Etag), It.IsAny<CancellationToken>()), Times.Once);
            _container.Verify(container => container.UpsertItemAsync(itemWithEtag2, new PartitionKey(itemWithEtag2.Id), It.Is<ItemRequestOptions>(options => options.IfMatchEtag == itemWithEtag2.Etag), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_MultipleItems_WhenIgnoreEtagIsTrueAndItemTypeIsNotAssignableToIItemWithEtag_UseDefaultEtagValueInUpsertOptions()
        {
            // Arrange
            TestItem item1 = new();
            TestItem item2 = new();

            IEnumerable<TestItem> testItems = new []
            {
                item1,
                item2
            };

            _containerProviderForTestItem.Setup(cp => cp.GetContainerAsync()).ReturnsAsync(_container.Object);

            // Act
            await RepositoryForItemWithoutETag.UpdateAsync(testItems, default, true);

            // Assert
            _container.Verify(
                container => container.UpsertItemAsync(item1, It.Is<PartitionKey>(key => key == new PartitionKey(item1.Id)),
                    It.Is<ItemRequestOptions>(options => options.IfMatchEtag == default),
                    It.IsAny<CancellationToken>()), Times.Once);

            _container.Verify(
                container => container.UpsertItemAsync(item2, It.Is<PartitionKey>(key => key == new PartitionKey(item2.Id)),
                    It.Is<ItemRequestOptions>(options => options.IfMatchEtag == default),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_MultipleItems_WhenIgnoreEtagIsFalseAndItemTypeIsNotAssignableToIItemWithEtag_UseDefaultEtagValueInUpsertOptions()
        {
            // Arrange
            TestItem item1 = new();
            TestItem item2 = new();

            IEnumerable<TestItem> testItems = new []
            {
                item1,
                item2
            };

            _containerProviderForTestItem.Setup(cp => cp.GetContainerAsync()).ReturnsAsync(_container.Object);

            // Act
            await RepositoryForItemWithoutETag.UpdateAsync(testItems);

            // Assert
            _container.Verify(
                container => container.UpsertItemAsync(item1, It.Is<PartitionKey>(key => key == new PartitionKey(item1.Id)),
                    It.Is<ItemRequestOptions>(options => options.IfMatchEtag == default),
                    It.IsAny<CancellationToken>()), Times.Once);

            _container.Verify(
                container => container.UpsertItemAsync(item2, It.Is<PartitionKey>(key => key == new PartitionKey(item2.Id)),
                    It.Is<ItemRequestOptions>(options => options.IfMatchEtag == default),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_MultipleItems_WhenIgnoreEtagIsTrueAndEtagIsNull_UseDefaultEtagValueInUpsertOptions()
        {
            // Arrange
            TestItemWithEtag itemWithEtag1 = new()
            {
                Etag = Guid.NewGuid().ToString()
            };

            TestItemWithEtag itemWithEtag2 = new()
            {
                Etag = null!
            };

            IEnumerable<TestItemWithEtag> testItemWithEtags = new []
            {
                itemWithEtag1,
                itemWithEtag2
            };

            _containerProviderForTestItemWithETag.Setup(cp => cp.GetContainerAsync()).ReturnsAsync(_container.Object);

            // Act
            await RepositoryForItemWithETag.UpdateAsync(testItemWithEtags, default, true);

            // Assert
            _container.Verify(
                container => container.UpsertItemAsync(itemWithEtag1, It.Is<PartitionKey>(key => key == new PartitionKey(itemWithEtag1.Id)),
                    It.Is<ItemRequestOptions>(options => options.IfMatchEtag == default),
                    It.IsAny<CancellationToken>()), Times.Once);

            _container.Verify(
                container => container.UpsertItemAsync(itemWithEtag2, It.Is<PartitionKey>(key => key == new PartitionKey(itemWithEtag2.Id)),
                    It.Is<ItemRequestOptions>(options => options.IfMatchEtag == default),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_MultipleItems_WhenIgnoreEtagIsFalseAndAEtagIsNull_UseDefaultEtagValueInUpsertOptions()
        {
            // Arrange
            TestItemWithEtag itemWithEtag1 = new()
            {
                Etag = Guid.NewGuid().ToString()
            };

            TestItemWithEtag itemWithEtag2 = new()
            {
                Etag = null!
            };

            IEnumerable<TestItemWithEtag> testItemWithEtags = new []
            {
                itemWithEtag1,
                itemWithEtag2
            };

            _containerProviderForTestItemWithETag.Setup(cp => cp.GetContainerAsync()).ReturnsAsync(_container.Object);

            // Act
            await RepositoryForItemWithETag.UpdateAsync(testItemWithEtags);

            // Assert
            _container.Verify(
                container => container.UpsertItemAsync(itemWithEtag1, new PartitionKey(itemWithEtag1.Id),
                    It.Is<ItemRequestOptions>(options => options.IfMatchEtag == itemWithEtag1.Etag),
                    It.IsAny<CancellationToken>()), Times.Once);

            _container.Verify(
                container => container.UpsertItemAsync(itemWithEtag2, new PartitionKey(itemWithEtag2.Id),
                    It.Is<ItemRequestOptions>(options => options.IfMatchEtag == default),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_Patch_WhenEtagIsDefault_UseDefaultEtagValueInPatchOptions()
        {
            // Arrange
            string id = Guid.NewGuid().ToString();

            _containerProviderForTestItemWithETag.Setup(cp => cp.GetContainerAsync()).ReturnsAsync(_container.Object);

            // Act
            await RepositoryForItemWithETag.UpdateAsync(id, _ => { });

            // Assert
            _container.Verify(
                container => container.PatchItemAsync<TestItemWithEtag>(id, It.Is<PartitionKey>(key => key == new PartitionKey(id)), It.IsAny<IReadOnlyList<PatchOperation>>(),
                    It.Is<PatchItemRequestOptions>(options => options.IfMatchEtag == default),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_Patch_WhenEtagIsEmpty_UseDefaultEtagValueInPatchOptions()
        {
            // Arrange
            string id = Guid.NewGuid().ToString();
            string etag = string.Empty;

            _containerProviderForTestItemWithETag.Setup(cp => cp.GetContainerAsync()).ReturnsAsync(_container.Object);

            // Act
            await RepositoryForItemWithETag.UpdateAsync(id, _ => { }, null, default, etag);

            // Assert
            _container.Verify(
                container => container.PatchItemAsync<TestItemWithEtag>(id, It.Is<PartitionKey>(key => key == new PartitionKey(id)), It.IsAny<IReadOnlyList<PatchOperation>>(),
                    It.Is<PatchItemRequestOptions>(options => options.IfMatchEtag == default),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_Patch_WhenEtagIsWhiteSpace_UseDefaultEtagValueInPatchOptions()
        {
            // Arrange
            string id = Guid.NewGuid().ToString();
            string etag = "         ";

            _containerProviderForTestItemWithETag.Setup(cp => cp.GetContainerAsync()).ReturnsAsync(_container.Object);

            // Act
            await RepositoryForItemWithETag.UpdateAsync(id, _ => { }, null, default, etag);

            // Assert
            _container.Verify(
                container => container.PatchItemAsync<TestItemWithEtag>(id, It.Is<PartitionKey>(key => key == new PartitionKey(id)), It.IsAny<IReadOnlyList<PatchOperation>>(),
                    It.Is<PatchItemRequestOptions>(options => options.IfMatchEtag == default),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_Patch_WhenEtagIsSet_UseSetEtagValueInPatchOptions()
        {
            // Arrange
            string id = Guid.NewGuid().ToString();
            string etag = Guid.NewGuid().ToString();

            _containerProviderForTestItemWithETag.Setup(cp => cp.GetContainerAsync()).ReturnsAsync(_container.Object);

            // Act
            await RepositoryForItemWithETag.UpdateAsync(id, _ => { }, null, default, etag);

            // Assert
            _container.Verify(
                container => container.PatchItemAsync<TestItemWithEtag>(id, It.Is<PartitionKey>(key => key == new PartitionKey(id)),It.IsAny<IReadOnlyList<PatchOperation>>(),
                    It.Is<PatchItemRequestOptions>(options => options.IfMatchEtag == etag),
                    It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    class MockExpressionProvider : IRepositoryExpressionProvider
    {
        public Expression<Func<TItem, bool>> Build<TItem>(Expression<Func<TItem, bool>>? predicate) where TItem : IItem
            => predicate!.Compose(item => item.Type == typeof(TItem).Name, Expression.AndAlso);

        public TItem CheckItem<TItem>(TItem item) where TItem : IItem
        {
            throw new NotImplementedException();
        }
    }
}