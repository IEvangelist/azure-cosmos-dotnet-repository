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
        readonly Mock<ICosmosContainerProvider<TestItem>> _containerProvider = new();
        readonly Mock<ICosmosQueryableProcessor> _queryableProcessor = new();
        readonly Mock<IOptionsMonitor<RepositoryOptions>> _options = new();
        readonly RepositoryOptions _repositoryOptions = new();
        readonly Mock<Container> _container = new();
        readonly IRepositoryExpressionProvider _expressionProvider = new MockExpressionProvider();


        public DefaultRepositoryTests()
        {
            _options.Setup(o => o.CurrentValue).Returns(_repositoryOptions);
        }

        private DefaultRepository<TestItem> Repository =>
            new(_options.Object,
                _containerProvider.Object,
                new NullLogger<DefaultRepository<TestItem>>(),
                _queryableProcessor.Object,
                _expressionProvider);


        [Fact]
        public async Task GetAsyncGivenExpressionQueriesContainerCorrectly()
        {
            //Arrange
            List<TestItem> items = new()
            {
                new(){Id = "a"},
                new(){Id = "c"},
                new(){Id = "ab"}
            };

            Expression<Func<TestItem, bool>> predicate = item => item.Id == "a" || item.Id == "ab";

            Expression<Func<TestItem, bool>> expectedPredicate = _expressionProvider.Build(predicate);

            IOrderedQueryable<TestItem> queryable = items.AsQueryable().Where(expectedPredicate).OrderBy(i => i.Id);
            _containerProvider.Setup(o => o.GetContainerAsync()).ReturnsAsync(_container.Object);

            _container
                .Setup(o => o.GetItemLinqQueryable<TestItem>(false, null, null, null))
                .Returns(queryable);

            _queryableProcessor.Setup(o => o.IterateAsync(queryable, It.IsAny<CancellationToken>()))
                .ReturnsAsync(items);

            //Act
            IEnumerable<TestItem> result = await Repository.GetAsync(predicate);

            //Assert
            Assert.Equal(items, result);
            Assert.Equal(2, queryable.ToList().Count);
        }
    }

    class MockExpressionProvider : IRepositoryExpressionProvider
    {
        public Expression<Func<TItem, bool>> Build<TItem>(Expression<Func<TItem, bool>> predicate) where TItem : IItem
            => predicate.Compose(item => item.Type == typeof(TItem).Name, Expression.AndAlso);
    }
}