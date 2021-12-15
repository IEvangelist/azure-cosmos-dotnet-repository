// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Extensions;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Processors;
using Microsoft.Azure.CosmosRepository.Providers;
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

        public DefaultRepositoryTests()
        {
            _options.Setup(o => o.CurrentValue).Returns(_repositoryOptions);
        }

        private DefaultRepository<TestItemWithEtag> RepositoryForItemWithETag =>
            new(_options.Object,
                _containerProviderForTestItemWithETag.Object,
                new NullLogger<DefaultRepository<TestItemWithEtag>>(),
                _queryableProcessor.Object,
                _expressionProvider);

        private DefaultRepository<TestItem> RepositoryForItemWithoutETag =>
            new(_options.Object,
                _containerProviderForTestItem.Object,
                new NullLogger<DefaultRepository<TestItem>>(),
                _queryableProcessor.Object,
                _expressionProvider);

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

            Expression<Func<TestItemWithEtag, bool>> expectedPredicate = _expressionProvider.Build(predicate);

            IOrderedQueryable<TestItemWithEtag> queryable = items.AsQueryable().Where(expectedPredicate).OrderBy(i => i.Id);
            _containerProviderForTestItemWithETag.Setup(o => o.GetContainerAsync()).ReturnsAsync(_container.Object);

            _container
                .Setup(o => o.GetItemLinqQueryable<TestItemWithEtag>(false, null, null, null))
                .Returns(queryable);

            _queryableProcessor.Setup(o => o.IterateAsync(queryable, It.IsAny<CancellationToken>()))
                .ReturnsAsync(items);

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

            Expression<Func<TestItemWithEtag, bool>> expectedPredicate = _expressionProvider.Build(predicate);

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
        public async Task UpdateAsync_SingleItem_WhenUseEtagIsTrueAndEtagIsProvided_UseEtagInUpsertOptions()
        {
            // Arrange
            TestItemWithEtag itemWithEtag = new()
            {
                Etag = Guid.NewGuid().ToString()
            };

            _containerProviderForTestItemWithETag.Setup(cp => cp.GetContainerAsync()).ReturnsAsync(_container.Object);

            // Act
            await RepositoryForItemWithETag.UpdateAsync(itemWithEtag, default, true);

            // Assert
            _container.Verify(container => container.UpsertItemAsync(itemWithEtag, new PartitionKey(itemWithEtag.Id), It.Is<ItemRequestOptions>(options => options.IfMatchEtag == itemWithEtag.Etag), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_SingleItem_WhenVerifyEtagIsTrueAndItemTypeIsNotAssignableToIItemWithEtag_ThrowXYZException()
        {
            // Arrange
            TestItem item = new();

            _containerProviderForTestItem.Setup(cp => cp.GetContainerAsync()).ReturnsAsync(_container.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => RepositoryForItemWithoutETag.UpdateAsync(item, default, true).AsTask());
        }

        [Fact]
        public async Task UpdateAsync_SingleItem_WhenVerifyEtagIsFalseAndItemTypeIsNotAssignableToIItemWithEtag_UseDefaultEtagValueInUpsertOptions()
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
        public async Task UpdateAsync_SingleItem_WhenVerifyEtagIsFalseAndEtagIsNull_UseDefaultEtagValueInUpsertOptions()
        {
            // Arrange
            TestItemWithEtag itemWithEtag = new()
            {
                Etag = null
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
        public async Task UpdateAsync_SingleItem_WhenVerifyEtagIsTrueAndEtagIsNull_UseDefaultEtagValueInUpsertOptions()
        {
            // Arrange
            TestItemWithEtag itemWithEtag = new()
            {
                Etag = null
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
    }

    class MockExpressionProvider : IRepositoryExpressionProvider
    {
        public Expression<Func<TItem, bool>> Build<TItem>(Expression<Func<TItem, bool>> predicate) where TItem : IItem
            => predicate.Compose(item => item.Type == typeof(TItem).Name, Expression.AndAlso);
    }
}