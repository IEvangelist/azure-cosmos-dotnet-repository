---
title: "Querying"
weight: 4
chapter: true
pre: "<b>4. </b>"
---
# Querying Data in the Cosmos Repository

The Cosmos Repository supports a rich set of methods for all sorts of query's that you might need to run. This includes basic filters, paging using skip & take, paging uses Cosmos DB continuation tokens, as well as, a specification pattern implementation pattern to do even more with queries.

## Basics Querying

The simplest way to get started with a query is to use the `_repository.GetAsync<TITem>` method this allows you to pass an expression used to filter your query.

```csharp
public static class PeopleRepositoryExtensions
{
    public static async Task<IEnumerable<Person>> GetPeopleOlderThan(
        this IRepository<Person> repository, DateTime date)
    {
        return await repository.GetAsync(p => p.BirthDate > date);
    }

    public static async Task<IEnumerable<Person>> GetPeopleWithoutMiddleNames(this IRepository<Person> repository)
    {
        IEnumerable<Person> peopleWithoutMiddleNames = 
            await repository.GetAsync(p => p.MiddleName == null);

        return peopleWithoutMiddleNames;
    }
}
```

{{% notice info %}}
The above example makes use of extensions methods. This can be a nice way to define queries using this library.
{{% /notice %}}


## Basic Paging

There are 3 ways to page data in the library. Below shows the simplest one. It makes use of skip & take, or limit and offset.

```csharp
public class PagingExamples
{
    public async Task BasicPageAsync(IRepository<Person> repository)
    {
        double totalCharge = 0;

        IPageQueryResult<Person> page = await repository.PageAsync(pageNumber: 1, pageSize: 25);

        while (page.HasNextPage)
        {
            foreach (Person person in page.Items)
            {
                Console.WriteLine(person);
            }

            totalCharge += page.Charge;
            page = await repository.PageAsync(pageNumber: page.PageNumber.Value + 1, pageSize: 25);

            Console.WriteLine($"Get page {page.PageNumber} 25 results cost {page.Charge}");
        }

        Console.WriteLine($"Total Charge {totalCharge} RU's");
    }
}
```

{{% notice warning %}}
This method if used on large data sets has an exponential RU charge. The further you read into the pages the more RUs this costs. 
Continuation token paging is preferred.
{{% /notice %}}

## Continuation Token Paging

The second way to page data using the library is making use of Cosmos DBs continuation tokens. This offers a more cost effective way to page data in Cosmos DB. Just provide the continuation token back the `.PageAsync` method to pick up where you left off.

```csharp
public class PagingExamples
{
    public async Task BasicScrollingAsync(IRepository<Person> repository)
    {
        double totalCharge = 0;
    
        IPage<Person> page = await repository.PageAsync(pageSize: 25, continuationToken: null);
    
        foreach (Person person in page.Items)
        {
            Console.WriteLine(person);
        }
    
        totalCharge += page.Charge;
    
        Console.WriteLine($"First 25 results cost {page.Charge}");
    
        page = await repository.PageAsync(pageSize: 25, continuationToken: page.Continuation);
    
        foreach (Person person in page.Items)
        {
            Console.WriteLine(person);
        }
    
        totalCharge += page.Charge;
        Console.WriteLine($"Second 25 results cost {page.Charge}");
    
        page = await repository.PageAsync(pageSize: 50, continuationToken: page.Continuation);
    
        foreach (Person person in page.Items)
        {
            Console.WriteLine(person);
        }
    
        totalCharge += page.Charge;
    
        Console.WriteLine($"Last 50 results cost {page.Charge}");
        Console.WriteLine($"Total Charge {totalCharge} RU's");
    }
}
```

## Advanced Continuation Token Paging

If you want to stream large amounts of data you can make use of the `IAsyncEnumerable<T>` type in C# to stream the pages to a consumer as they arrive.

```csharp
public class ParcelRepository
{    
    private readonly IRepository<Parcel> _parcelCosmosRepository;    

    public ParcelRepository(IRepository<Parcel> parcelCosmosRepository) =>
        _parcelCosmosRepository = parcelCosmosRepository;        

    public async IAsyncEnumerable<IParcel> StreamParcelsWithDeliveryRegionId(
        string deliveryRegionId,
        int max,
        int chunkSize = 25,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int collected = 0;
        bool hasMoreResults = true;
        string? token = null;

        Expression<Func<Parcel, bool>> expression = parcel =>
            parcel.PartitionKey == deliveryRegionId &&
            parcel.Status == ParcelStatus.Inducted;

        while (hasMoreResults && collected < max)
        {
            var page = await _parcelCosmosRepository
                    .PageAsync(expression, chunkSize, token, cancellationToken);

            token = page.Continuation;
            hasMoreResults = page.Continuation is not null;

            foreach (var item in page.Items)
            {
                if (collected < max)
                {
                    yield return item;   
                }
                
                collected++;
            }
        }
    }
}
```

