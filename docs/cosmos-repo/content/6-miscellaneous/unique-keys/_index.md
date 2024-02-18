---
title: "Unique Keys Policies"
weight: 2
---

Azure Cosmos DB provides the ability to define a set of properties on your JSON item that can be combined to form a unique key for a given partition. These are defined in the form of unique key policies. You can define multiple policies for the same partition key.

## Explanation of Unique Keys

This is better explained with an example. Let's for example say we are storing people in a Cosmos container. The people items are partitioned by by the county that they live in. So the partition key would be `/county`. Let's say for example we can't have a person with the same name and age in a county. This means that we need to build up a unique key based on the person's name and age.

## Single Key Policy Example

In order to do this in the Azure Cosmos Repository you could define a class as below. This class decorates properties with attributes `[UniqueKey]` to define properties as unique keys.

```csharp
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Attributes;

namespace Sample.Models

public class Person : Item
{
    [UniqueKey(propertyPath: "/firstName")]
    public string FirstName { get; set; }

    [UniqueKey(propertyPath: "/age")]
    public int Age { get; set; }

    public string County { get; set; }

    public string FavouriteColor  { get; set; }

    protected override string GetPartitionKeyValue() => 
        County;

    public Person(
        string firstName, 
        int age, 
        string county, 
        string favouriteColor)
    {
        FirstName = firstName;
        Age = age;
        County = county;
        FavouriteColor = favouriteColor;
    }
}
```

Adding to the above example the code below shows when a violation in this policy should occur should occur.

```csharp
IRepository<Person> repository = _serviceProvider.GetRequiredService<IRepository<Person>>();

Person bobInYorkshire = new Person("bob", 20, "Yorkshire", "Blue");
repository.CreateAsync(bobInYorkshire);

// This is in a different partition key range, so we can have another bob with the age of 20.
Person bobInMerseyside = new Person("bob", 20, "Merseyside", "Green");
repository.CreateAsync(bobInYorkshire);

// This is bob with a different age in Yorkshire so this does not violate the policy.
Person bobInYorkshireWhoIs22 = new Person("bob", 22, "Yorkshire", "Red");
repository.CreateAsync(bobInYorkshireWhoIs22);

try
{
    //Adding another bob in Yorkshire violates the key.
    Person bobInYorkshire = new Person("bob", 20, "Yorkshire", "Yellow");
    repository.CreateAsync(bobInYorkshire);
}
catch (CosmosException e) when (e.StatusCode is HttpStatusCode.Conflict)
{
    //This is a violation of the key!
}
```

> The fact that these key policies are scoped to a single partition key range is very important! You can read more on this on the [Official Cosmos DB Docs](https://docs.microsoft.com/en-us/azure/cosmos-db/unique-keys).

## Multiple Key Policies Example
It is also possible in Cosmos DB to define multiple unique key policies for a single partition key range. It is possible to extend the above example by adding a policy that we cannot have people in the same county with the same favourite colour either. This can be implemented by adding another `[UniqueKey]` attribute onto the `FavouriteColour` property.

> Pay attention to the fact when defining the `[UniqueKey]` attributes we are grouping paths to different key names. This allows the policies to split. One for name and age and another for the favourite colour.

```csharp
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Attributes;

namespace Sample.Models

public class Person : Item
{
    [UniqueKey(keyName: "nameAndAgePolicyKeyName", propertyPath: "/firstName")]
    public string FirstName { get; set; }

    [UniqueKey(keyName: "nameAndAgePolicyKeyName", propertyPath: "/age")]
    public int Age { get; set; }

    public string County { get; set; }

    [UniqueKey(keyName: "favouriteColorPolicyKeyName", propertyPath: "/favouriteColor")]
    public string FavouriteColor  { get; set; }

    protected override string GetPartitionKeyValue() => 
        County;

    public Person(
        string firstName, 
        int age, 
        string county, 
        string favouriteColor)
    {
        FirstName = firstName;
        Age = age;
        County = county;
        FavouriteColor = favouriteColor;
    }
}
```

This defines two 3 unique keys split across 2 separate policies two paths are under the `nameAndAgePolicyKeyName` policy and one path is under the `favouriteColorPolicyKeyName` policy. See the example below when validation would occur in this example.

```csharp

IRepository<Person> repository = _serviceProvider.GetRequiredService<IRepository<Person>>();

Person bobInYorkshire = new Person("bob", 20, "Yorkshire", "Blue");
repository.CreateAsync(bobInYorkshire);

// This is in a different partition key range, so we can have another bob with the age of 20.
Person bobInMerseyside = new Person("bob", 20, "Merseyside", "Green");
repository.CreateAsync(bobInYorkshire);

// This is bob with a different age in Yorkshire so this does not violate the policy.
Person bobInYorkshireWhoIs22 = new Person("bob", 22, "Yorkshire", "Red");
repository.CreateAsync(bobInYorkshireWhoIs22);

try
{
    //Fred does have a unique name and age, but he cannot also as Red as his favourite color.
    Person fredInYorkshireWhoAlsoLikeRed = new Person("fred", 30, "Yorkshire", "Red");
    repository.CreateAsync(fredInYorkshireWhoAlsoLikeRed);
}
catch (CosmosException e) when (e.StatusCode is HttpStatusCode.Conflict)
{
    //This is a violation of the key!
}
```
