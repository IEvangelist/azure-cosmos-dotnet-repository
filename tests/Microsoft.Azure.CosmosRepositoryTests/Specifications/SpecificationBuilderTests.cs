// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

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

        Assert.Single(spec.WhereExpressions);
        Assert.Single(spec.OrderExpressions);
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