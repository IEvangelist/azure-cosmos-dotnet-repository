# Unique Key Policies

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

    public UniqueKeyPolicyItem(
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
Person bobInMerseyside = new Person("bob", 20, "Merseyside", "Red");
repository.CreateAsync(bobInYorkshire);

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

## Multiple Key Policies Example

