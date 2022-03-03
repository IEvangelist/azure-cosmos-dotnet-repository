---
title: "Specification Pattern"
weight: 1
---

The Azure Cosmos Repository also provides a way to query data based on a set of specifications. These are defined as classes. The inspiration for this implementation was taken from [`ardalis/Specification`](https://github.com/ardalis/Specification) on Github. This provides a similar mechanism for entity framework.

## Simple Specification

The specification below queries for all products in a category ordered by there price.

```csharp

public class ProductSpecificationExamples
{
    private class ProductsPriceLowestToHighestInCategory : DefaultSpecification<Product>
    {
        public ProductsPriceLowestToHighestInCategory(string categoryId) =>
            Query
                .Where(x => x.PartitionKey == categoryId)
                .OrderBy(x => x.Price);
    }

    public async Task<IQueryResult<Product>> RunDemoAsync(IRepository<Product> repository)
    {
        IQueryResult<Product> orderedProducts =
                await _productsRepository.QueryAsync(new ProductsPriceLowestToHighestInCategory("Clothing"));

        return orderedProducts;
    }
}
```

## Specifications - Ordered

A specification can also contain more than one ordering clause. You can order by one property then by another.

```csharp
public class ProductSpecificationAdvancedOrderingExamples
{
    private class ProductsPriceLowestToHighestThenByName : DefaultSpecification<Product>
    {
        public ProductsPriceLowestToHighestInCategory(string categoryId) =>
            Query
                .OrderBy(x => x.Price)
                .ThenByDescending(x => x.Name);
    }

    public async Task<IQueryResult<Product>> RunDemoAsync(IRepository<Product> repository)
    {
        IQueryResult<Product> orderedProducts =
                await _productsRepository.QueryAsync(new ProductsPriceLowestToHighestThenByName());

        return orderedProducts;
    }
}
```

{{% notice warning %}}
If you want to use the `.ThenBy` or `.ThenByDescending` methods you will need to add composite index to the fields you are querying on. If not you will get an exception.
{{% /notice %}}