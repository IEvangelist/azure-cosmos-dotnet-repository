// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests;

class Person : Item, IItemWithEtag
{
    [JsonProperty("_etag")] public string Etag { get; set; } = null!;

    public string Name { get; set; }

    public Person(string name)
    {
        Name = name;
    }
}

class Dog : FullItem
{
    public string Breed { get; }
    public string Name { get; private set; }

    protected override string GetPartitionKeyValue() => Breed;


    public Dog(string breed, string name = "dasher")
    {
        Breed = breed;
        Name = name;
    }
}

class DogComparer : IEqualityComparer<Dog?>
{
    public bool Equals(Dog? x, Dog? y) => x?.Id == y?.Id;

    public int GetHashCode(Dog? obj) => obj != null ? obj.Id.GetHashCode() : 0;
}

class RootObject : Item
{
    public string Type1 { get; set; } = null!;

    public NestedObject NestedObject { get; set; } = null!;
}

class NestedObject
{
    public string Property1 { get; set; } = null!;

    public int Property2 { get; set; }
}

class InvalidSerialisableArguments
{
    public InvalidSerialisableArguments(string partitionKey, string property1)
    {
        PartitionKey = partitionKey;
        Property1 = property1;
    }

    public string PartitionKey { get; }
    public string Property1 { get; }
}

class InvalidSerialisable : InvalidSerialisableBase
{
    public InvalidSerialisable(InvalidSerialisableArguments args) : base(args.PartitionKey)
    {
        Property1 = args.Property1;
    }

    public string Property1 { get; }
}

class ValidInvalidSerialisable : InvalidSerialisableBase
{
    public ValidInvalidSerialisable(string partitionKey, string property1) : base(partitionKey)
    {
        Property1 = property1;
    }

    public string Property1 { get; }
}

class InvalidSerialisableBase : Item
{
    public InvalidSerialisableBase(string id)
    {
        PartitionKey = id;
    }

    public string PartitionKey { get; }
}

public class InMemoryRepositoryTests
{
    private readonly InMemoryRepository<Person> _personRepository = new();
    private readonly InMemoryRepository<Dog> _dogRepository = new();
    private readonly InMemoryRepository<RootObject> _rootObjectRepository = new();
    private readonly InMemoryRepository<InvalidSerialisable> _invalidSerializableRepository = new();

    [Fact]
    public async Task GetAsync_InvalidSerializable_ThrowsNullException()
    {
        //Arrange
        InvalidSerialisableArguments args = new("id", "property1");
        InvalidSerialisable invalidSerializable = new(args);
        InMemoryStorage.GetDictionary<InvalidSerialisable>()[invalidSerializable.Id] =
            JsonConvert.SerializeObject(invalidSerializable);

        //Act
        //Assert
        await Assert.ThrowsAsync<NullReferenceException>(() =>
            _invalidSerializableRepository.GetAsync(invalidSerializable.Id).AsTask());
    }

    [Fact]
    public async Task GetAsync_IdThatDoesNotExist_ThrowsCosmosException()
    {
        //Arrange
        //Act
        //Assert
        CosmosException ex = await Assert.ThrowsAsync<CosmosException>(() =>
            _personRepository.GetAsync(Guid.NewGuid().ToString()).AsTask());

        Assert.Equal(HttpStatusCode.NotFound, ex.StatusCode);
    }

    [Fact]
    public async Task GetAsync_IdAndPartitionKeyStringNotFound_ThrowsCosmosException()
    {
        //Arrange
        //Act
        //Assert
        CosmosException ex = await Assert.ThrowsAsync<CosmosException>(() =>
            _dogRepository.GetAsync(Guid.NewGuid().ToString(), "cocker-spaniel").AsTask());

        Assert.Equal(HttpStatusCode.NotFound, ex.StatusCode);
    }

    [Fact]
    public async Task GetAsync_IdAndPartitionKeyObjectNotFound_ThrowsCosmosException()
    {
        //Arrange
        //Act
        //Assert
        CosmosException ex = await Assert.ThrowsAsync<CosmosException>(() =>
            _dogRepository.GetAsync(Guid.NewGuid().ToString(), new PartitionKey("cocker-spaniel")).AsTask());

        Assert.Equal(HttpStatusCode.NotFound, ex.StatusCode);
    }

    [Fact]
    public async Task GetAsync_IdThatExists_GetsItem()
    {
        //Arrange
        Person item = new("joe") { Id = Guid.NewGuid().ToString(), Type = nameof(Person) };
        InMemoryStorage.GetDictionary<Person>().TryAddAsJson(item.Id, item);

        //Act
        Person person = await _personRepository.GetAsync(item.Id);


        //Assert
        Assert.Equal(item.Id, person.Id);
        Assert.Equal(item.Type, person.Type);
        Assert.Equal(item.Name, person.Name);
    }

    [Fact]
    public async Task GetAsync_IdAndPartitionKeyStringExists_GetsItem()
    {
        //Arrange
        Dog item = new("cocker-spanel") { Id = Guid.NewGuid().ToString(), Type = nameof(Dog) };
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(item.Id, item);

        //Act
        Dog dog = await _dogRepository.GetAsync(item.Id, item.Breed);

        //Assert
        Assert.Equal(item.Id, dog.Id);
        Assert.Equal(item.Type, dog.Type);
        Assert.Equal(item.Breed, dog.Breed);
    }

    [Fact]
    public async Task GetAsync_IdAndPartitionKeyObjectExists_GetsItem()
    {
        Dog item = new("cocker-spanel") { Id = Guid.NewGuid().ToString(), Type = nameof(Dog) };
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(item.Id, item);

        //Act
        Dog dog = await _dogRepository.GetAsync(item.Id, new PartitionKey(item.Breed));

        //Assert
        Assert.Equal(item.Id, dog.Id);
        Assert.Equal(item.Type, dog.Type);
        Assert.Equal(item.Breed, dog.Breed);
    }

    [Fact]
    public async Task GetAsync_PredicateThatDoesNotMatch_ReturnsEmptyList()
    {
        //Arrange
        Person person = new("joe");
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(person.Id, person);

        //Act
        IEnumerable<Person> people = await _personRepository.GetAsync(p => p.Name == "fred");

        //Assert
        Assert.False(people.Any());
    }

    [Fact]
    public async Task GetAsync_PredicateThatDoesMatch_ReturnsItemInList()
    {
        //Arrange
        Person person = new("joe");
        InMemoryStorage.GetDictionary<Person>().TryAddAsJson(person.Id, person);

        //Act
        IEnumerable<Person> people = await _personRepository.GetAsync(p => p.Name == "joe");

        //Assert
        var enumerable = people.ToList();
        Assert.True(enumerable.Any());
        Person item = enumerable.First(x => x.Id == person.Id);
        Assert.Equal(person.Name, item.Name);
        Assert.Equal(person.Id, item.Id);
        Assert.Equal(person.Type, item.Type);
    }

    [Fact]
    public async Task CreateAsync_WhenTypeCannotBeDeserialised_StoresTheRecordAndThrowsCosmosException()
    {
        //Arrange
        InvalidSerialisableArguments args = new("id", "property1");
        InvalidSerialisable invalidSerialisable = new(args);

        //Act
        //Assert
        await Assert.ThrowsAsync<NullReferenceException>(() =>
            _invalidSerializableRepository.CreateAsync(invalidSerialisable).AsTask());
        Assert.True(InMemoryStorage.GetDictionary<InvalidSerialisable>().ContainsKey(invalidSerialisable.Id));
        Assert.Equal(args.Property1,
            _invalidSerializableRepository
                .DeserializeItem<ValidInvalidSerialisable>(
                    InMemoryStorage.GetDictionary<InvalidSerialisable>()[invalidSerialisable.Id]).Property1);
        Assert.Equal(args.PartitionKey,
            _invalidSerializableRepository
                .DeserializeItem<ValidInvalidSerialisable>(
                    InMemoryStorage.GetDictionary<InvalidSerialisable>()[invalidSerialisable.Id]).PartitionKey);
    }

    [Fact]
    public async Task CreateAsync_ItemWhereIdAlreadyExists_ThrowsCosmosException()
    {
        //Arrange
        Person item = new("joe") { Id = Guid.NewGuid().ToString(), Type = nameof(Person) };
        InMemoryStorage.GetDictionary<Person>().TryAddAsJson(item.Id, item);

        //Act
        CosmosException ex = await Assert.ThrowsAsync<CosmosException>(() =>
            _personRepository.CreateAsync(new Person("joe") { Id = item.Id, Type = nameof(Person) }).AsTask());

        Assert.Equal(HttpStatusCode.Conflict, ex.StatusCode);
    }

    [Fact]
    public async Task CreateAsync_SingleItemWithTimeStamps_WhenCreatedTimeStampHasNotBeenSet_ShouldSetCreatedTimeStamp()
    {
        // Arrange
        Dog dog = new("labradoodle", "Thor");

        // Act
        Dog returnedDog = await _dogRepository.CreateAsync(dog);

        // Assert
        Dog addedDog = await _dogRepository.GetAsync(dog.Id, dog.Breed);

        Assert.Equal(returnedDog.Name, addedDog.Name);
        Assert.Equal(returnedDog.Id, addedDog.Id);
        Assert.Equal(returnedDog.Type, addedDog.Type);

        Assert.Equal(addedDog.Name, dog.Name);
        Assert.Equal(addedDog.Id, dog.Id);
        Assert.Equal(addedDog.Type, dog.Type);

        Assert.NotNull(dog.CreatedTimeUtc);
        Assert.NotNull(returnedDog.CreatedTimeUtc);
        Assert.NotNull(addedDog.CreatedTimeUtc);
        Assert.InRange(dog.CreatedTimeUtc!.Value, DateTime.UtcNow.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));
        Assert.InRange(returnedDog.CreatedTimeUtc!.Value, DateTime.UtcNow.AddSeconds(-1),
            DateTime.UtcNow.AddSeconds(1));
        Assert.InRange(addedDog.CreatedTimeUtc!.Value, DateTime.UtcNow.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public async Task
        CreateAsync_SingleItemWithTimeStamps_WhenCreatedTimeStampHasAlreadyBeenSet_ShouldNotSetCreatedTimeStamp()
    {
        // Arrange
        DateTime expectedCreatedTimeUtc = DateTime.UtcNow.AddDays(-7);
        Dog dog = new("labradoodle", "Thor")
        {
            CreatedTimeUtc = expectedCreatedTimeUtc
        };

        // Act
        Dog returnedDog = await _dogRepository.CreateAsync(dog);

        // Assert
        Dog addedDog = await _dogRepository.GetAsync(dog.Id, dog.Breed);

        Assert.Equal(returnedDog.Name, addedDog.Name);
        Assert.Equal(returnedDog.Id, addedDog.Id);
        Assert.Equal(returnedDog.Type, addedDog.Type);

        Assert.Equal(addedDog.Name, dog.Name);
        Assert.Equal(addedDog.Id, dog.Id);
        Assert.Equal(addedDog.Type, dog.Type);

        var calculatedTs = _dogRepository.CurrentTs;
        DateTime calculatedTsDate = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(calculatedTs);
        Assert.InRange(returnedDog.LastUpdatedTimeUtc, calculatedTsDate.AddSeconds(-1), calculatedTsDate.AddSeconds(1));
        Assert.InRange(addedDog.LastUpdatedTimeUtc, calculatedTsDate.AddSeconds(-1), calculatedTsDate.AddSeconds(1));

        Assert.NotNull(dog.CreatedTimeUtc);
        Assert.NotNull(returnedDog.CreatedTimeUtc);
        Assert.NotNull(addedDog.CreatedTimeUtc);
        Assert.Equal(expectedCreatedTimeUtc, dog.CreatedTimeUtc!.Value);
        Assert.Equal(expectedCreatedTimeUtc, returnedDog.CreatedTimeUtc!.Value);
        Assert.Equal(expectedCreatedTimeUtc, addedDog.CreatedTimeUtc!.Value);
    }

    [Fact]
    public async Task CreateAsync_Item_CreatesItem()
    {
        //Arrange
        Person item = new("joe") { Id = Guid.NewGuid().ToString(), Type = nameof(Person) };

        //Act
        Person person = await _personRepository.CreateAsync(item);

        Person addedPerson = await _personRepository.GetAsync(item.Id);

        Assert.Equal(item.Name, person.Name);
        Assert.Equal(item.Id, person.Id);
        Assert.Equal(item.Type, person.Type);

        Assert.Equal(item.Name, addedPerson.Name);
        Assert.Equal(item.Id, addedPerson.Id);
        Assert.Equal(item.Type, addedPerson.Type);

        Assert.True(!string.IsNullOrWhiteSpace(addedPerson.Etag) && addedPerson.Etag != Guid.Empty.ToString());
    }

    [Fact]
    public async Task CreateAsync_ManyItems_CreatesAllItems()
    {
        //Arrange
        List<Person> items =
        [
            new("joe") { Id = Guid.NewGuid().ToString(), Type = nameof(Person) },
            new("bill") { Id = Guid.NewGuid().ToString(), Type = nameof(Person) },
            new("fred") { Id = Guid.NewGuid().ToString(), Type = nameof(Person) },
        ];

        //Act
        IEnumerable<Person> people = (await _personRepository.CreateAsync(items)).ToList();

        foreach (Person item in items)
        {
            Person addedPerson = InMemoryStorage.GetDictionary<Person>().Values
                .Select(_personRepository.DeserializeItem).First(i => i.Id == item.Id);
            Person person = people.First(i => i.Id == item.Id);

            Assert.Equal(item.Name, person.Name);
            Assert.Equal(item.Id, person.Id);
            Assert.Equal(item.Type, person.Type);

            Assert.Equal(item.Name, addedPerson.Name);
            Assert.Equal(item.Id, addedPerson.Id);
            Assert.Equal(item.Type, addedPerson.Type);

            Assert.True(!string.IsNullOrWhiteSpace(addedPerson.Etag) && addedPerson.Etag != Guid.Empty.ToString());
        }
    }

    [Fact]
    public async Task
        CreateAsync_ManyItemsWhereOneHasIdThatAlreadyExists_CreatesInitalItemsThenThrowsCosmosException()
    {
        //Arrange
        List<Person> items =
        [
            new("joe") { Id = Guid.NewGuid().ToString(), Type = nameof(Person) },
            new("bill") { Id = Guid.NewGuid().ToString(), Type = nameof(Person) },
            new("fred") { Id = Guid.NewGuid().ToString(), Type = nameof(Person) },
        ];

        Person badPerson = new("copy") { Id = items.First().Id, Type = nameof(Person) };
        items.Add(badPerson);

        //Act
        CosmosException ex =
            await Assert.ThrowsAsync<CosmosException>(() => _personRepository.CreateAsync(items).AsTask());

        Assert.Equal(HttpStatusCode.Conflict, ex.StatusCode);

        foreach (Person item in items.Where(i => i.Name != badPerson.Name))
        {
            Person person = InMemoryStorage.GetDictionary<Person>().Values.Select(_personRepository.DeserializeItem)
                .First(i => i.Id == item.Id);

            Assert.Equal(item.Name, person.Name);
            Assert.Equal(item.Id, person.Id);
            Assert.Equal(item.Type, person.Type);

            Assert.True(!string.IsNullOrWhiteSpace(person.Etag) && person.Etag != Guid.Empty.ToString());
        }
    }


    [Fact]
    public async Task DeleteAsync_IdThatDoesNotExist_ThrowsCosmosException()
    {
        //Arrange
        //Act
        CosmosException ex =
            await Assert.ThrowsAsync<CosmosException>(
                () => _personRepository.DeleteAsync("does-not-exist").AsTask());

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, ex.StatusCode);
    }

    [Fact]
    public async Task DeleteAsync_IdPartitionKeyThatDoesNotExist_ThrowsCosmosException()
    {
        //Arrange
        //Act
        CosmosException ex = await Assert.ThrowsAsync<CosmosException>(
            () => _personRepository.DeleteAsync("does-not-exist", new PartitionKey("does-not-exist")).AsTask());

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, ex.StatusCode);
    }

    [Fact]
    public async Task DeleteAsync_ItemWithIdThatExists_DeletesItem()
    {
        //Arrange
        Person person = new("joe");
        InMemoryStorage.GetDictionary<Person>().TryAddAsJson(person.Id, person);

        //Act
        await _personRepository.DeleteAsync(person.Id);

        //Assert
        Person? oldPerson = await _personRepository.TryGetAsync(person.Id);
        Assert.Null(oldPerson);
    }

    [Fact]
    public async Task DeleteAsync_ItemWithIdAndPartitionKeyThatExists_DeletesItem()
    {
        //Arrange
        Dog dog = new("cocker spaniel");
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog.Id, dog);

        //Act
        await _dogRepository.DeleteAsync(dog.Id, dog.Breed);

        //Assert
        Assert.False(InMemoryStorage.GetDictionary<Dog>().Any());
    }

    [Fact]
    public async Task UpdateAsync_ItemThatDoesNotExist_AddsItem()
    {
        //Arrange
        Person item = new("joe") { Id = Guid.NewGuid().ToString(), Type = nameof(Person) };

        //Act
        Person person = await _personRepository.UpdateAsync(item);

        Person addedPerson = await _personRepository.GetAsync(item.Id);

        Assert.Equal(item.Name, person.Name);
        Assert.Equal(item.Id, person.Id);
        Assert.Equal(item.Type, person.Type);

        Assert.Equal(item.Name, addedPerson.Name);
        Assert.Equal(item.Id, addedPerson.Id);
        Assert.Equal(item.Type, addedPerson.Type);

        Assert.True(!string.IsNullOrWhiteSpace(person.Etag) && person.Etag != Guid.Empty.ToString());
    }

    [Fact]
    public async Task UpdateAsync_WhereEtagsMatch_Updates()
    {
        //Arrange
        var originalEtag = Guid.NewGuid().ToString();
        Person item = new("joe") { Id = Guid.NewGuid().ToString(), Type = nameof(Person), Etag = originalEtag };
        InMemoryStorage.GetDictionary<Person>().TryAddAsJson(item.Id, item);
        Person updateItem = new("joe2") { Id = item.Id, Type = nameof(Person), Etag = originalEtag };

        //Act
        Person person = await _personRepository.UpdateAsync(updateItem, false, default);

        Person addedPerson = await _personRepository.GetAsync(person.Id);

        Assert.Equal(updateItem.Name, person.Name);
        Assert.Equal(updateItem.Id, person.Id);
        Assert.Equal(updateItem.Type, person.Type);

        Assert.Equal(updateItem.Name, addedPerson.Name);
        Assert.Equal(updateItem.Id, addedPerson.Id);
        Assert.Equal(updateItem.Type, addedPerson.Type);

        Assert.True(!string.IsNullOrWhiteSpace(addedPerson.Etag) && addedPerson.Etag != Guid.Empty.ToString() &&
                    addedPerson.Etag != originalEtag);
    }

    [Fact]
    public async Task UpdateAsync_WhereEtagsDontMatch_ThrowsCosmosException()
    {
        //Arrange
        var updateEtag = Guid.NewGuid().ToString();
        var storedEtag = Guid.NewGuid().ToString();

        Person storedItem = new("joe") { Id = Guid.NewGuid().ToString(), Type = nameof(Person), Etag = storedEtag };
        InMemoryStorage.GetDictionary<Person>().TryAddAsJson(storedItem.Id, storedItem);

        Person updateItem = new("joe") { Id = storedItem.Id, Type = nameof(Person), Etag = updateEtag };

        //Act
        CosmosException cosmosException =
            await Assert.ThrowsAsync<CosmosException>(() => _personRepository.UpdateAsync(updateItem).AsTask());
        Assert.Equal(HttpStatusCode.PreconditionFailed, cosmosException.StatusCode);
    }

    [Fact]
    public async Task UpdateAsync_WhereValueEtagIsNull_Updates()
    {
        //Arrange
        var originalEtag = Guid.NewGuid().ToString();
        Person item = new("joe") { Id = Guid.NewGuid().ToString(), Type = nameof(Person), Etag = originalEtag };
        InMemoryStorage.GetDictionary<Person>().TryAddAsJson(item.Id, item);
        Person updateItem = new("joe2") { Id = item.Id, Type = nameof(Person), Etag = null! };

        //Act
        Person person = await _personRepository.UpdateAsync(updateItem, false, default);

        Person addedPerson = await _personRepository.GetAsync(person.Id);

        Assert.Equal(updateItem.Name, person.Name);
        Assert.Equal(updateItem.Id, person.Id);
        Assert.Equal(updateItem.Type, person.Type);

        Assert.Equal(updateItem.Name, addedPerson.Name);
        Assert.Equal(updateItem.Id, addedPerson.Id);
        Assert.Equal(updateItem.Type, addedPerson.Type);

        Assert.True(!string.IsNullOrWhiteSpace(addedPerson.Etag) && addedPerson.Etag != Guid.Empty.ToString() &&
                    addedPerson.Etag != originalEtag);
    }

    [Fact]
    public async Task UpdateAsync_ManyItems_UpdatesAllItems()
    {
        //Arrange
        var originalEtag = Guid.NewGuid().ToString();
        List<Person> items =
        [
            new("joe") { Id = Guid.NewGuid().ToString(), Type = nameof(Person), Etag = originalEtag },
            new("bill") { Id = Guid.NewGuid().ToString(), Type = nameof(Person), Etag = originalEtag },
            new("fred") { Id = Guid.NewGuid().ToString(), Type = nameof(Person), Etag = originalEtag },
        ];

        List<Person> itemsUpdate = [];

        foreach (Person item in items)
        {
            InMemoryStorage.GetDictionary<Person>().TryAddAsJson(item.Id, item);
            Person itemUpdate = new($"{item.Name}Updated")
            {
                Id = item.Id,
                Type = item.Type,
                Etag = item.Etag
            };
            itemsUpdate.Add(itemUpdate);
        }

        //Act
        IEnumerable<Person> people = (await _personRepository.UpdateAsync(itemsUpdate, false, default)).ToList();

        foreach (Person item in items)
        {
            Person updatedPerson = InMemoryStorage.GetDictionary<Person>().Values
                .Select(_personRepository.DeserializeItem).First(i => i.Id == item.Id);
            Person person = people.First(i => i.Id == item.Id);
            Person itemUpdate = itemsUpdate.First(i => i.Id == item.Id);

            Assert.NotEqual(item.Name, updatedPerson.Name);
            Assert.Equal(item.Id, updatedPerson.Id);
            Assert.Equal(item.Type, updatedPerson.Type);

            Assert.Equal(person.Name, updatedPerson.Name);
            Assert.Equal(person.Id, updatedPerson.Id);
            Assert.Equal(person.Type, updatedPerson.Type);

            Assert.Equal(itemUpdate.Name, updatedPerson.Name);
            Assert.Equal(itemUpdate.Id, updatedPerson.Id);
            Assert.Equal(itemUpdate.Type, updatedPerson.Type);

            Assert.True(!string.IsNullOrWhiteSpace(updatedPerson.Etag) && updatedPerson.Etag != Guid.Empty.ToString() &&
                        updatedPerson.Etag != originalEtag);
        }
    }

    [Fact]
    public async Task UpdateAsync_ManyItemsWhereEtagsDontMatch_ThrowsCosmosException()
    {
        //Arrange
        var originalEtag = Guid.NewGuid().ToString();
        List<Person> items =
        [
            new("joe") { Id = Guid.NewGuid().ToString(), Type = nameof(Person), Etag = originalEtag },
            new("bill") { Id = Guid.NewGuid().ToString(), Type = nameof(Person), Etag = originalEtag },
            new("fred") { Id = Guid.NewGuid().ToString(), Type = nameof(Person), Etag = originalEtag },
        ];

        List<Person> itemsUpdate = [];

        foreach (Person item in items)
        {
            InMemoryStorage.GetDictionary<Person>().TryAddAsJson(item.Id, item);
            Person itemUpdate = new($"{item.Name}Updated")
            {
                Id = item.Id,
                Type = item.Type,
                Etag = Guid.NewGuid().ToString()
            };
            itemsUpdate.Add(itemUpdate);
        }

        //Act & Assert
        await Assert.ThrowsAsync<CosmosException>(() =>
            _personRepository.UpdateAsync(itemsUpdate, false, default).AsTask());
    }

    [Fact]
    public async Task UpdateAsync_ItemThatExists_UpdatesItem()
    {
        //Arrange
        var originalEtag = Guid.NewGuid().ToString();
        Dog dog = new("labrador");
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog.Id, dog);
        Dog updatedDog = new("labradoodle") { Id = dog.Id };

        //Act
        Dog returnedDog = await _dogRepository.UpdateAsync(updatedDog);

        Dog addedDog = await _dogRepository.GetAsync(updatedDog.Id, updatedDog.Breed);

        Assert.Equal(updatedDog.Name, returnedDog.Name);
        Assert.Equal(dog.Id, returnedDog.Id);
        Assert.Equal(updatedDog.Type, returnedDog.Type);

        Assert.Equal(updatedDog.Name, addedDog.Name);
        Assert.Equal(dog.Id, addedDog.Id);
        Assert.Equal(updatedDog.Type, addedDog.Type);

        Assert.True(!string.IsNullOrWhiteSpace(addedDog.Etag) && addedDog.Etag != Guid.Empty.ToString() &&
                    addedDog.Etag != originalEtag);

        var calculatedTs = _dogRepository.CurrentTs;
        DateTime calculatedTsDate = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(calculatedTs);
        Assert.InRange(returnedDog.LastUpdatedTimeUtc, calculatedTsDate.AddSeconds(-1), calculatedTsDate.AddSeconds(1));
        Assert.InRange(addedDog.LastUpdatedTimeUtc, calculatedTsDate.AddSeconds(-1), calculatedTsDate.AddSeconds(1));
    }

    [Fact]
    public async Task ExistsAsync_PointReadWhenItemsExists_ReturnsTrue()
    {
        //Arrange
        Person person = new("joe");

        InMemoryStorage.GetDictionary<Person>().TryAddAsJson(person.Id, person);

        //Act
        var exists = await _personRepository.ExistsAsync(person.Id);

        //Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_PointReadWithPartitionKeyItemsExists_ReturnsTrue()
    {
        //Arrange
        Dog dog = new("cocker spaniel");
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog.Id, dog);

        //Act
        var exists = await _dogRepository.ExistsAsync(dog.Id, dog.Breed);

        //Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_PointReadWhenDoesNotItemsExists_ReturnsFalse()
    {
        //Arrange
        Person person = new("joe");

        InMemoryStorage.GetDictionary<Person>().TryAddAsJson(person.Id, person);

        //Act
        var exists = await _personRepository.ExistsAsync("fred");

        //Assert
        Assert.False(exists);
    }


    [Fact]
    public async Task ExistsAsync_CountQueryWithItemsThatMatch_ReturnsTrue()
    {
        //Arrange
        Dog dog1 = new("cocker spaniel");
        Dog dog2 = new("cocker spaniel");
        Dog dog3 = new("golden retriever");

        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog1.Id, dog1);
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog2.Id, dog2);
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog3.Id, dog3);

        //Act
        var exists = await _dogRepository.ExistsAsync(d => d.Breed == "cocker spaniel" || d.Id == dog3.Id);

        //Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task CountAsync_CountAllItems_ReturnsTotal()
    {
        //Arrange
        Dog dog1 = new("cocker spaniel");
        Dog dog2 = new("cocker spaniel");
        Dog dog3 = new("golden retriever");

        InMemoryStorage.GetDictionary<Dog>().Clear();
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog1.Id, dog1);
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog2.Id, dog2);
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog3.Id, dog3);

        //Act
        var count = await _dogRepository.CountAsync();

        //Assert
        Assert.Equal(3, count);
    }

    [Fact]
    public async Task CountAsync_CountAllOnEmptyContainer_ReturnsZero()
    {
        //Act
        InMemoryStorage.GetDictionary<Dog>().Clear();
        var count = await _dogRepository.CountAsync();

        //Assert
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task CountAsync_CountQueryWithItemsThatMatch_ReturnsTotal()
    {
        //Arrange
        Dog dog1 = new("cocker spaniel");
        Dog dog2 = new("cocker spaniel");
        Dog dog3 = new("golden retriever");

        InMemoryStorage.GetDictionary<Dog>().Clear();
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog1.Id, dog1);
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog2.Id, dog2);
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog3.Id, dog3);

        //Act
        var count = await _dogRepository.CountAsync(d => d.Breed == "cocker spaniel");

        //Assert
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task CountAsync_CountQueryWithItemsThatDoNotMatch_ReturnsZero()
    {
        //Arrange
        Dog dog1 = new("cocker spaniel");
        Dog dog2 = new("cocker spaniel");
        Dog dog3 = new("golden retriever");

        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog1.Id, dog1);
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog2.Id, dog2);
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog3.Id, dog3);

        //Act
        var count = await _dogRepository.CountAsync(d => d.Breed == "maltese");

        //Assert
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task UpdateAsync_PropertiesToPatch_UpdatesValues()
    {
        //Arrange
        Dog dog = new("labrador", "fred");
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog.Id, dog);

        //Act
        await _dogRepository.UpdateAsync(dog.Id, builder => builder.Replace(d => d.Name, "kenny"), dog.Breed);

        //Assert
        Dog addedDog = await _dogRepository.GetAsync(dog.Id, dog.Breed);
        Assert.Equal("kenny", addedDog.Name);

        var calculatedTs = _dogRepository.CurrentTs;
        DateTime calculatedTsDate = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(calculatedTs);
        Assert.InRange(addedDog.LastUpdatedTimeUtc, calculatedTsDate.AddSeconds(-1), calculatedTsDate.AddSeconds(1));
        Assert.InRange(addedDog.LastUpdatedTimeRaw, calculatedTs - 1, calculatedTs + 1);
    }

    [Fact]
    public async Task UpdateAsync_PropertiesToPatch_WhenEtagMatches_UpdatesValues()
    {
        //Arrange
        var originalEtag = Guid.NewGuid().ToString();
        Person person = new("steve")
        {
            Etag = originalEtag
        };
        InMemoryStorage.GetDictionary<Person>().TryAddAsJson(person.Id, person);

        //Act
        await _personRepository.UpdateAsync(person.Id, builder => builder.Replace(p => p.Name, "kenny"), default,
            originalEtag, default);

        //Assert
        Person updatedPerson = await _personRepository.GetAsync(person.Id);
        Assert.Equal("kenny", updatedPerson.Name);
        Assert.Equal(person.Id, updatedPerson.Id);
        Assert.True(!string.IsNullOrWhiteSpace(updatedPerson.Etag) && updatedPerson.Etag != Guid.Empty.ToString() &&
                    updatedPerson.Etag != originalEtag);
    }

    [Fact]
    public async Task UpdateAsync_PropertiesToPatch_WhenEtagIsNull_UpdatesValues()
    {
        //Arrange
        var originalEtag = Guid.NewGuid().ToString();
        Person person = new("steve")
        {
            Etag = originalEtag
        };
        InMemoryStorage.GetDictionary<Person>().TryAddAsJson(person.Id, person);

        //Act
        await _personRepository.UpdateAsync(person.Id, builder => builder.Replace(p => p.Name, "kenny"), default, null,
            default);

        //Assert
        Person updatedPerson = _personRepository.DeserializeItem(InMemoryStorage.GetDictionary<Person>().First().Value);
        Assert.Equal("kenny", updatedPerson.Name);
        Assert.Equal(person.Id, updatedPerson.Id);
        Assert.True(!string.IsNullOrWhiteSpace(updatedPerson.Etag) && updatedPerson.Etag != Guid.Empty.ToString() &&
                    updatedPerson.Etag != originalEtag);
    }


    [Fact]
    public async Task UpdateAsync_PropertiesToPatch_WhenEtagIsEmpty_UpdatesValues()
    {
        //Arrange
        var originalEtag = Guid.NewGuid().ToString();
        Person person = new("steve")
        {
            Etag = originalEtag
        };
        InMemoryStorage.GetDictionary<Person>().TryAddAsJson(person.Id, person);

        //Act
        await _personRepository.UpdateAsync(person.Id, builder => builder.Replace(p => p.Name, "kenny"), default,
            string.Empty, default);

        //Assert
        Person updatedPerson = await _personRepository.GetAsync(person.Id);
        Assert.Equal("kenny", updatedPerson.Name);
        Assert.Equal(person.Id, updatedPerson.Id);
        Assert.True(!string.IsNullOrWhiteSpace(updatedPerson.Etag) && updatedPerson.Etag != Guid.Empty.ToString() &&
                    updatedPerson.Etag != originalEtag);
    }

    [Fact]
    public async Task UpdateAsync_PropertiesToPatch_WhenEtagIsWhiteSpace_UpdatesValues()
    {
        //Arrange
        var originalEtag = Guid.NewGuid().ToString();
        Person person = new("steve")
        {
            Etag = originalEtag
        };
        InMemoryStorage.GetDictionary<Person>().TryAddAsJson(person.Id, person);

        //Act
        await _personRepository.UpdateAsync(person.Id, builder => builder.Replace(p => p.Name, "kenny"), default,
            "            ", default);

        //Assert
        Person updatedPerson = await _personRepository.GetAsync(person.Id);
        Assert.Equal("kenny", updatedPerson.Name);
        Assert.Equal(person.Id, updatedPerson.Id);
        Assert.True(!string.IsNullOrWhiteSpace(updatedPerson.Etag) && updatedPerson.Etag != Guid.Empty.ToString() &&
                    updatedPerson.Etag != originalEtag);
    }

    [Fact]
    public async Task UpdateAsync_PropertiesToPatch_WhenEtagDoesNotMatch_ThrowsCosmosException()
    {
        //Arrange
        var originalEtag = Guid.NewGuid().ToString();
        Person person = new("steve")
        {
            Etag = originalEtag
        };
        InMemoryStorage.GetDictionary<Person>().TryAddAsJson(person.Id, person);

        //Act & Assert
        CosmosException cosmosException = await Assert.ThrowsAsync<CosmosException>(() =>
            _personRepository.UpdateAsync(person.Id, builder => builder.Replace(p => p.Name, "kenny"), default,
                Guid.NewGuid().ToString(), default).AsTask());
        Assert.Equal(HttpStatusCode.PreconditionFailed, cosmosException.StatusCode);


        Person updatedPerson = await _personRepository.GetAsync(person.Id);
        Assert.Equal(person.Name, updatedPerson.Name);
        Assert.Equal(person.Id, updatedPerson.Id);
    }

    [Fact]
    public async Task UpdateAsync_NestedObjectToPatch_UpdatesValues()
    {
        //Arrange
        RootObject root = new()
        {
            Id = Guid.NewGuid().ToString(),
            Type1 = "ABC",
            NestedObject = new NestedObject
            {
                Property1 = "prop1",
                Property2 = 55
            }
        };

        InMemoryStorage.GetDictionary<RootObject>().TryAddAsJson(root.Id, root);

        //Act
        await _rootObjectRepository.UpdateAsync(root.Id, builder =>
            builder.Replace(x => x.Type1, "CBA")
                .Replace(x => x.NestedObject, new NestedObject
                {
                    Property1 = "prop2",
                    Property2 = 2
                }));

        //Assert
        RootObject deserialisedItem =
            _rootObjectRepository.DeserializeItem(InMemoryStorage.GetDictionary<RootObject>().First().Value);
        Assert.Equal("CBA", deserialisedItem.Type1);
        Assert.Equal("prop2", deserialisedItem.NestedObject.Property1);
        Assert.Equal(2, deserialisedItem.NestedObject.Property2);
    }

    [Fact]
    public async Task UpdateAsync_PropertiesInNestedObjectToPatch_UpdatesValues()
    {
        //Arrange
        RootObject root = new()
        {
            Id = Guid.NewGuid().ToString(),
            NestedObject = new NestedObject
            {
                Property1 = "prop1",
                Property2 = 55
            }
        };

        InMemoryStorage.GetDictionary<RootObject>().TryAddAsJson(root.Id, root);

        //Act
        await _rootObjectRepository.UpdateAsync(
            root.Id,
            builder =>
                builder.Replace(x => x.NestedObject.Property1, "prop2")
                    .Replace(x => x.NestedObject.Property2, 2));

        //Assert
        RootObject deserialisedItem =
            _rootObjectRepository.DeserializeItem(InMemoryStorage.GetDictionary<RootObject>().First().Value);
        Assert.Equal("prop2", deserialisedItem.NestedObject.Property1);
        Assert.Equal(2, deserialisedItem.NestedObject.Property2);
    }

    [Fact]
    public async Task PageAsync_PredicateThatDoesNotMatch_ReturnsEmptyList()
    {
        //Arrange
        Dog dog1 = new("cocker spaniel");
        Dog dog2 = new("cocker spaniel");
        Dog dog3 = new("cocker spaniel");
        InMemoryStorage.GetDictionary<Dog>().Clear();
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog1.Id, dog1);
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog2.Id, dog2);
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog3.Id, dog3);
        var pageSize = 2;
        var pageNumber = 2;

        //Act
        IPageQueryResult<Dog> dogs =
            await _dogRepository.PageAsync(d => d.Breed == "golden retriever", pageNumber, pageSize);

        //Assert
        var enumerable = dogs.Items.ToList();
        Assert.False(enumerable.Any());
    }

    [Fact]
    public async Task PageAsync_ReturnTotalIsFalse_ReturnsCorrectItemsAndTotalIsNullAndCountHasNotBeenCalled()
    {
        //Arrange
        Dog dog1 = new("cocker spaniel");
        Dog dog2 = new("cocker spaniel");
        Dog dog3 = new("cocker spaniel");
        Dog dog4 = new("cocker spaniel");
        Dog dog5 = new("cocker spaniel");
        Dog dog6 = new("cocker spaniel");
        Dog dog7 = new("golden retriever");
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog1.Id, dog1);
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog2.Id, dog2);
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog3.Id, dog3);
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog4.Id, dog4);
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog5.Id, dog5);
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog6.Id, dog6);
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog7.Id, dog7);
        var pageSize = 2;
        var pageNumber = 2;

        var expectedList = InMemoryStorage.GetDictionary<Dog>().Select(d => JsonConvert.DeserializeObject<Dog>(d.Value))
            .Where(d => d?.Breed == "cocker spaniel")
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToList();

        //Act
        IPageQueryResult<Dog> dogs =
            await _dogRepository.PageAsync(d => d.Breed == "cocker spaniel", pageNumber, pageSize);

        //Assert
        var enumerable = dogs.Items.ToList();
        Assert.True(enumerable.Any());
        Assert.Equal(expectedList, enumerable, new DogComparer());
        Assert.Equal(dogs.PageNumber, pageNumber);
        Assert.True(dogs.HasPreviousPage);
        Assert.Equal(1, dogs.PreviousPageNumber);
        Assert.True(dogs.TotalPages is null);
        Assert.True(dogs.NextPageNumber is null);
        Assert.True(dogs.HasNextPage is null);
    }

    [Fact]
    public async Task PageAsync_ReturnTotalIsTrue_ReturnsCorrectItemsAndTotalIsNotNull()
    {
        //Arrange
        Dog dog1 = new("cocker spaniel");
        Dog dog2 = new("cocker spaniel");
        Dog dog3 = new("cocker spaniel");
        Dog dog4 = new("cocker spaniel");
        Dog dog5 = new("cocker spaniel");
        Dog dog6 = new("cocker spaniel");
        Dog dog7 = new("golden retriever");
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog1.Id, dog1);
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog2.Id, dog2);
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog3.Id, dog3);
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog4.Id, dog4);
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog5.Id, dog5);
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog6.Id, dog6);
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog7.Id, dog7);
        var pageSize = 2;
        var pageNumber = 2;

        var expectedList = InMemoryStorage.GetDictionary<Dog>().Select(d => JsonConvert.DeserializeObject<Dog>(d.Value))
            .Where(d => d?.Breed == "cocker spaniel")
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToList();

        //Act
        IPageQueryResult<Dog> dogs =
            await _dogRepository.PageAsync(d => d.Breed == "cocker spaniel", pageNumber, pageSize);

        //Assert
        var enumerable = dogs.Items.ToList();
        Assert.True(enumerable.Any());
        Assert.Equal(expectedList, enumerable, new DogComparer());
        Assert.Equal(dogs.PageNumber, pageNumber);
        Assert.True(dogs.HasPreviousPage);
        Assert.Equal(1, dogs.PreviousPageNumber);
        Assert.True(dogs.TotalPages is null);
        Assert.True(dogs.NextPageNumber is null);
        Assert.True(dogs.HasNextPage is null);
    }

    [Fact]
    public async Task PageAsync_SpecificationThatDoesNotMatch_ReturnsEmptyList()
    {
        //Arrange
        Dog dog1 = new("cocker spaniel");
        Dog dog2 = new("cocker spaniel");
        Dog dog3 = new("cocker spaniel");
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog1.Id, dog1);
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog2.Id, dog2);
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog3.Id, dog3);
        var pageSize = 2;
        var pageNumber = 2;

        DogSpecification specification = new("golden retriever", 2, 2);

        //Act
        IPageQueryResult<Dog> dogs =
            await _dogRepository.PageAsync(d => d.Breed == "golden retriever", pageNumber, pageSize, true);

        //Assert
        var enumerable = dogs.Items.ToList();
        Assert.False(enumerable.Any());
    }

    [Fact]
    public async Task PageAsync_SpecificationThatDoesMatch_ReturnsItemInList()
    {
        //Arrange
        Dog dog1 = new("cocker spaniel");
        Dog dog2 = new("cocker spaniel");
        Dog dog3 = new("cocker spaniel");
        Dog dog4 = new("cocker spaniel");
        Dog dog5 = new("cocker spaniel");
        Dog dog6 = new("cocker spaniel");
        Dog dog7 = new("golden retriever");
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog1.Id, dog1);
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog2.Id, dog2);
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog3.Id, dog3);
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog4.Id, dog4);
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog5.Id, dog5);
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog6.Id, dog6);
        InMemoryStorage.GetDictionary<Dog>().TryAddAsJson(dog7.Id, dog7);
        var pageSize = 2;
        var pageNumber = 2;
        DogSpecification specification = new("cocker spaniel", 2, 2);

        var expectedList = InMemoryStorage.GetDictionary<Dog>().Select(d => JsonConvert.DeserializeObject<Dog>(d.Value))
            .Where(d => d?.Breed == "cocker spaniel")
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToList();

        //Act
        IPage<Dog> dogs = await _dogRepository.QueryAsync(specification);

        //Assert
        var enumerable = dogs.Items.ToList();
        Assert.True(enumerable.Any());
        Assert.Equal(expectedList, enumerable, new DogComparer());
    }

    [Fact]
    public async Task ReadWriteInterfaces_AddedViaDi_PersistsTheSameData()
    {
        // Arrange
        IServiceProvider provider = new ServiceCollection()
            .AddInMemoryCosmosRepository()
            .BuildServiceProvider();

        var dog = new Dog("cocker spaniel", "nelson");

        IWriteOnlyRepository<Dog> writeOnlyRepository = provider.GetRequiredService<IWriteOnlyRepository<Dog>>();
        IReadOnlyRepository<Dog> readOnlyRepository = provider.GetRequiredService<IReadOnlyRepository<Dog>>();
        IRepository<Dog> normalRepository = provider.GetRequiredService<IRepository<Dog>>();

        //Act
        await writeOnlyRepository.CreateAsync(dog);

        //Assert
        Dog? dogViaReadonlyRepository = await readOnlyRepository.TryGetAsync(dog.Id, dog.Breed);
        Assert.NotNull(dogViaReadonlyRepository);

        Dog? dogViaNormalRepository = await normalRepository.TryGetAsync(dog.Id, dog.Breed);
        Assert.NotNull(dogViaNormalRepository);
    }

    internal class DogSpecification : OffsetByPageNumberSpecification<Dog>
    {
        internal DogSpecification(string breed, int pageNumber, int pageSize)
        {
            Query.Where(d => d.Breed == breed);
            Query.PageSize(pageNumber);
            Query.PageNumber(pageSize);
        }
    }
}