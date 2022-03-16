---
title: "Items"
weight: 3
chapter: true
pre: "<b>3. </b>"
---

# Items

The library provides a set of base types that can be used to tell the library that you want that model to be stored in Cosmos DB. These are know as `Item` types.

The simplest item types is the standard base `Item`. See an example of using `Item` below.

> See the rest of this section which covers the rest of the `Item` types.

```csharp
public class Product : Item
{
    public string Name { get; set; }

    public string CategoryId { get; set; }

    public string PartitionKey { get; set; }

    public double Price { get; set; }

    public StockInformation Stock { get; set; }

    protected override string GetPartitionKeyValue() =>
        CategoryId;
}
```