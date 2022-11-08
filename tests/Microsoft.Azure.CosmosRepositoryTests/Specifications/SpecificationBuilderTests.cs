// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Linq;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Specification;
using Microsoft.Azure.CosmosRepository.Specification.Builder;
using Microsoft.Azure.CosmosRepositoryTests.Stubs;
using Xunit;

namespace Microsoft.Azure.CosmosRepositoryTests.Specifications;

public class SpecificationBuilderTests
{
    [Fact]
    public void SpecificationBuilder_Query_BuildsCorrectExpressions()
    {
        DefaultSpecification<TestItem> spec = new();
        SpecificationBuilder<TestItem, IQueryResult<TestItem>> builder = new(spec);

        builder.Where(x => x.Id != string.Empty);
        builder.OrderBy(x => x.CreatedTimeUtc!);
        builder.PageNumber(5);
        builder.PageSize(25);

        Assert.Equal(1, spec.WhereExpressions.Count);
        Assert.Equal(1, spec.OrderExpressions.Count);
        Assert.Equal(25, spec.PageSize);
        Assert.Equal(5, spec.PageNumber);
    }

    [Fact]
    public void SpecificationBuilder_QueryWithOrderByAndThenBy_BuildsCorrectExpressions()
    {
        DefaultSpecification<TestItem> spec = new();
        SpecificationBuilder<TestItem, IQueryResult<TestItem>> builder = new(spec);

        builder
            .OrderBy(x => x.Id)
            .ThenBy(x => x.CreatedTimeUtc!);

        Assert.Equal(2, spec.OrderExpressions.Count);
        Assert.NotNull(spec.OrderExpressions.FirstOrDefault(x => x.OrderType is OrderTypeEnum.OrderBy));
        Assert.NotNull(spec.OrderExpressions.FirstOrDefault(x => x.OrderType is OrderTypeEnum.ThenBy));
    }
}