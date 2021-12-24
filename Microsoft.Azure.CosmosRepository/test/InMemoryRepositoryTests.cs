// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;
using Xunit;
using Microsoft.Azure.CosmosRepositoryTests.Extensions;
using Microsoft.Azure.CosmosRepository.Paging;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Azure.CosmosRepositoryTests
{
    class Person : Item, IItemWithEtag
    {
        [JsonProperty("_etag")]
        public string Etag { get; set; }

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

    class DogComparer : IEqualityComparer<Dog>
    {
        public bool Equals([AllowNull] Dog x, [AllowNull] Dog y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode([DisallowNull] Dog obj)
        {
            return obj?.Id.GetHashCode() ?? 0;
        }
    }

    class RootObject : Item
    {
        public string Type1 { get; set; }

        public NestedObject NestedObject { get; set; }
    }

    class NestedObject
    {
        public string Property1 { get; set; }

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
        private readonly InMemoryRepository<Person> _personRepository;
        private readonly InMemoryRepository<Dog> _dogRepository;
        private readonly InMemoryRepository<RootObject> _rootObjectRepository;
        private readonly InMemoryRepository<InvalidSerialisable> _invalidSerialisableRepository;

        public InMemoryRepositoryTests()
        {
            _personRepository = new InMemoryRepository<Person>();
            _dogRepository = new InMemoryRepository<Dog>();
            _rootObjectRepository = new InMemoryRepository<RootObject>();
            _invalidSerialisableRepository = new InMemoryRepository<InvalidSerialisable>();
        }

        [Fact]
        public async Task GetAsync_InvalidSerializable_ThrowsNullException()
        {
            //Arrange
            InvalidSerialisableArguments args = new("id", "property1");
            InvalidSerialisable invalidSerializable = new(args);
            _invalidSerialisableRepository.Items[invalidSerializable.Id] = JsonConvert.SerializeObject(invalidSerializable);

            //Act
            //Assert
            await Assert.ThrowsAsync<NullReferenceException>(() =>
                _invalidSerialisableRepository.GetAsync(invalidSerializable.Id).AsTask());
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
            _personRepository.Items.TryAddAsJson(item.Id, item);

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
            _dogRepository.Items.TryAddAsJson(item.Id, item);

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
            _dogRepository.Items.TryAddAsJson(item.Id, item);

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
            _personRepository.Items.TryAddAsJson(person.Id, person);

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
            _personRepository.Items.TryAddAsJson(person.Id, person);

            //Act
            IEnumerable<Person> people = await _personRepository.GetAsync(p => p.Name == "joe");

            //Assert
            List<Person> enumerable = people.ToList();
            Assert.True(enumerable.Any());
            Person item = enumerable.First();
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
                _invalidSerialisableRepository.CreateAsync(invalidSerialisable).AsTask());
            Assert.True(_invalidSerialisableRepository.Items.ContainsKey(invalidSerialisable.Id));
            Assert.Equal(args.Property1, _invalidSerialisableRepository.DeserializeItem<ValidInvalidSerialisable>(_invalidSerialisableRepository.Items[invalidSerialisable.Id]).Property1);
            Assert.Equal(args.PartitionKey, _invalidSerialisableRepository.DeserializeItem<ValidInvalidSerialisable>(_invalidSerialisableRepository.Items[invalidSerialisable.Id]).PartitionKey);
        }

        [Fact]
        public async Task CreateAsync_ItemWhereIdAlreadyExists_ThrowsCosmosException()
        {
            //Arrange
            Person item = new("joe") { Id = Guid.NewGuid().ToString(), Type = nameof(Person) };
            _personRepository.Items.TryAddAsJson(item.Id, item);

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
            Dog addedDog = _dogRepository.DeserializeItem(_dogRepository.Items.Values.First());

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
            Assert.InRange(returnedDog.CreatedTimeUtc!.Value, DateTime.UtcNow.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));
            Assert.InRange(addedDog.CreatedTimeUtc!.Value, DateTime.UtcNow.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));
        }

        [Fact]
        public async Task CreateAsync_SingleItemWithTimeStamps_WhenCreatedTimeStampHasAlreadyBeenSet_ShouldNotSetCreatedTimeStamp()
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
            Dog addedDog = _dogRepository.DeserializeItem(_dogRepository.Items.Values.First());

            Assert.Equal(returnedDog.Name, addedDog.Name);
            Assert.Equal(returnedDog.Id, addedDog.Id);
            Assert.Equal(returnedDog.Type, addedDog.Type);

            Assert.Equal(addedDog.Name, dog.Name);
            Assert.Equal(addedDog.Id, dog.Id);
            Assert.Equal(addedDog.Type, dog.Type);

            long calculatedTs = _dogRepository.CurrentTs;
            DateTime calculatedTsDate = new DateTime(1970, 1, 1, 0, 0,0, 0).AddSeconds(calculatedTs);
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

            Person addedPerson = _personRepository.DeserializeItem(_personRepository.Items.Values.First());

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
            List<Person> items = new()
            {
                new("joe") { Id = Guid.NewGuid().ToString(), Type = nameof(Person) },
                new("bill") { Id = Guid.NewGuid().ToString(), Type = nameof(Person) },
                new("fred") { Id = Guid.NewGuid().ToString(), Type = nameof(Person) },
            };

            //Act
            IEnumerable<Person> people = (await _personRepository.CreateAsync(items)).ToList();

            foreach (Person item in items)
            {
                Person addedPerson = _personRepository.Items.Values.Select(_personRepository.DeserializeItem).First(i => i.Id == item.Id);
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
            List<Person> items = new()
            {
                new("joe") { Id = Guid.NewGuid().ToString(), Type = nameof(Person) },
                new("bill") { Id = Guid.NewGuid().ToString(), Type = nameof(Person) },
                new("fred") { Id = Guid.NewGuid().ToString(), Type = nameof(Person) },
            };

            Person badPerson = new("copy") { Id = items.First().Id, Type = nameof(Person) };
            items.Add(badPerson);

            //Act
            CosmosException ex =
                await Assert.ThrowsAsync<CosmosException>(() => _personRepository.CreateAsync(items).AsTask());

            Assert.Equal(HttpStatusCode.Conflict, ex.StatusCode);

            foreach (Person item in items.Where(i => i.Name != badPerson.Name))
            {
                Person person = _personRepository.Items.Values.Select(_personRepository.DeserializeItem).First(i => i.Id == item.Id);

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
            _personRepository.Items.TryAddAsJson(person.Id, person);

            //Act
            await _personRepository.DeleteAsync(person.Id);

            //Assert
            Assert.False(_personRepository.Items.Any());
        }

        [Fact]
        public async Task DeleteAsync_ItemWithIdAndPartitionKeyThatExists_DeletesItem()
        {
            //Arrange
            Dog dog = new("cocker spaniel");
            _dogRepository.Items.TryAddAsJson(dog.Id, dog);

            //Act
            await _dogRepository.DeleteAsync(dog.Id, dog.Breed);

            //Assert
            Assert.False(_dogRepository.Items.Any());
        }

        [Fact]
        public async Task UpdateAsync_ItemThatDoesNotExist_AddsItem()
        {
            //Arrange
            Person item = new("joe") { Id = Guid.NewGuid().ToString(), Type = nameof(Person) };

            //Act
            Person person = await _personRepository.UpdateAsync(item);

            Person addedPerson = _personRepository.DeserializeItem(_personRepository.Items.Values.First());

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
            string originalEtag = Guid.NewGuid().ToString();
            Person item = new("joe") { Id = Guid.NewGuid().ToString(), Type = nameof(Person), Etag = originalEtag };
            _personRepository.Items.TryAddAsJson(item.Id, item);
            Person updateItem = new("joe2") { Id = item.Id, Type = nameof(Person), Etag = originalEtag };

            //Act
            Person person = await _personRepository.UpdateAsync(updateItem, default, false);

            Person addedPerson = _personRepository.DeserializeItem(_personRepository.Items.Values.First());

            Assert.Equal(updateItem.Name, person.Name);
            Assert.Equal(updateItem.Id, person.Id);
            Assert.Equal(updateItem.Type, person.Type);

            Assert.Equal(updateItem.Name, addedPerson.Name);
            Assert.Equal(updateItem.Id, addedPerson.Id);
            Assert.Equal(updateItem.Type, addedPerson.Type);

            Assert.True(!string.IsNullOrWhiteSpace(addedPerson.Etag) && addedPerson.Etag != Guid.Empty.ToString() && addedPerson.Etag != originalEtag);
        }

        [Fact]
        public async Task UpdateAsync_WhereEtagsDontMatch_ThrowsCosmosException()
        {
            //Arrange
            string updateEtag = Guid.NewGuid().ToString();
            string storedEtag = Guid.NewGuid().ToString();

            Person storedItem = new("joe") { Id = Guid.NewGuid().ToString(), Type = nameof(Person), Etag = storedEtag };
            _personRepository.Items.TryAddAsJson(storedItem.Id, storedItem);

            Person updateItem = new("joe") { Id = storedItem.Id, Type = nameof(Person), Etag = updateEtag };

            //Act
            CosmosException cosmosException = await Assert.ThrowsAsync<CosmosException>(() => _personRepository.UpdateAsync(updateItem).AsTask());
            Assert.Equal(HttpStatusCode.PreconditionFailed, cosmosException.StatusCode);
        }

        [Fact]
        public async Task UpdateAsync_ManyItems_UpdatesAllItems()
        {
            //Arrange
            string originalEtag = Guid.NewGuid().ToString();
            List<Person> items = new()
            {
                new("joe") { Id = Guid.NewGuid().ToString(), Type = nameof(Person), Etag = originalEtag },
                new("bill") { Id = Guid.NewGuid().ToString(), Type = nameof(Person), Etag = originalEtag },
                new("fred") { Id = Guid.NewGuid().ToString(), Type = nameof(Person), Etag = originalEtag },
            };

            List<Person> itemsUpdate = new();

            foreach (Person item in items)
            {
                _personRepository.Items.TryAddAsJson(item.Id, item);
                Person itemUpdate = new($"{item.Name}Updated")
                {
                    Id = item.Id,
                    Type = item.Type,
                    Etag = item.Etag
                };
                itemsUpdate.Add(itemUpdate);
            }

            //Act
            IEnumerable<Person> people = (await _personRepository.UpdateAsync(itemsUpdate, default, false)).ToList();

            foreach (Person item in items)
            {
                Person updatedPerson = _personRepository.Items.Values.Select(_personRepository.DeserializeItem).First(i => i.Id == item.Id);
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

                Assert.True(!string.IsNullOrWhiteSpace(updatedPerson.Etag) && updatedPerson.Etag != Guid.Empty.ToString() && updatedPerson.Etag != originalEtag);
            }
        }

        [Fact]
        public async Task UpdateAsync_ManyItemsWhereEtagsDontMatch_ThrowsCosmosException()
        {
            //Arrange
            string originalEtag = Guid.NewGuid().ToString();
            List<Person> items = new()
            {
                new("joe") { Id = Guid.NewGuid().ToString(), Type = nameof(Person), Etag = originalEtag },
                new("bill") { Id = Guid.NewGuid().ToString(), Type = nameof(Person), Etag = originalEtag },
                new("fred") { Id = Guid.NewGuid().ToString(), Type = nameof(Person), Etag = originalEtag },
            };

            List<Person> itemsUpdate = new();

            foreach (Person item in items)
            {
                _personRepository.Items.TryAddAsJson(item.Id, item);
                Person itemUpdate = new($"{item.Name}Updated")
                {
                    Id = item.Id,
                    Type = item.Type,
                    Etag = Guid.NewGuid().ToString()
                };
                itemsUpdate.Add(itemUpdate);
            }

            //Act & Assert
            await Assert.ThrowsAsync<CosmosException>(() => _personRepository.UpdateAsync(itemsUpdate, default, false).AsTask());
        }

        [Fact]
        public async Task UpdateAsync_ItemThatExists_UpdatesItem()
        {
            //Arrange
            string originalEtag = Guid.NewGuid().ToString();
            Dog dog = new("labrador");
            _dogRepository.Items.TryAddAsJson(dog.Id, dog);
            Dog updatedDog = new("labradoodle") {Id = dog.Id};

            //Act
            Dog returnedDog = await _dogRepository.UpdateAsync(updatedDog);

            Dog addedDog = _dogRepository.DeserializeItem(_dogRepository.Items.Values.First());

            Assert.Equal(updatedDog.Name, returnedDog.Name);
            Assert.Equal(dog.Id, returnedDog.Id);
            Assert.Equal(updatedDog.Type, returnedDog.Type);

            Assert.Equal(updatedDog.Name, addedDog.Name);
            Assert.Equal(dog.Id, addedDog.Id);
            Assert.Equal(updatedDog.Type, addedDog.Type);

            Assert.True(!string.IsNullOrWhiteSpace(addedDog.Etag) && addedDog.Etag != Guid.Empty.ToString() && addedDog.Etag != originalEtag);

            long calculatedTs = _dogRepository.CurrentTs;
            DateTime calculatedTsDate = new DateTime(1970, 1, 1, 0, 0,0, 0).AddSeconds(calculatedTs);
            Assert.InRange(returnedDog.LastUpdatedTimeUtc, calculatedTsDate.AddSeconds(-1), calculatedTsDate.AddSeconds(1));
            Assert.InRange(addedDog.LastUpdatedTimeUtc, calculatedTsDate.AddSeconds(-1), calculatedTsDate.AddSeconds(1));
        }

        [Fact]
        public async Task ExistsAsync_PointReadWhenItemsExists_ReturnsTrue()
        {
            //Arrange
            Person person = new("joe");

            _personRepository.Items.TryAddAsJson(person.Id, person);

            //Act
            bool exists = await _personRepository.ExistsAsync(person.Id);

            //Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task ExistsAsync_PointReadWithPartitionKeyItemsExists_ReturnsTrue()
        {
            //Arrange
            Dog dog = new("cocker spaniel");
            _dogRepository.Items.TryAddAsJson(dog.Id, dog);

            //Act
            bool exists = await _dogRepository.ExistsAsync(dog.Id, dog.Breed);

            //Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task ExistsAsync_PointReadWhenDoesNotItemsExists_ReturnsFalse()
        {
            //Arrange
            Person person = new("joe");

            _personRepository.Items.TryAddAsJson(person.Id, person);

            //Act
            bool exists = await _personRepository.ExistsAsync("fred");

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

            _dogRepository.Items.TryAddAsJson(dog1.Id, dog1);
            _dogRepository.Items.TryAddAsJson(dog2.Id, dog2);
            _dogRepository.Items.TryAddAsJson(dog3.Id, dog3);

            //Act
            bool exists = await _dogRepository.ExistsAsync(d => d.Breed == "cocker spaniel" || d.Id == dog3.Id);

            //Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task UpdateAsync_PropertiesToPatch_UpdatesValues()
        {
            //Arrange
            Dog dog = new("labrador", "fred");
            _dogRepository.Items.TryAddAsJson(dog.Id, dog);

            //Act
            await _dogRepository.UpdateAsync(dog.Id, builder => builder.Replace(d => d.Name, "kenny"), dog.Breed);

            //Assert
            Dog addedDog = _dogRepository.DeserializeItem(_dogRepository.Items.First().Value);
            Assert.Equal("kenny", addedDog.Name);

            long calculatedTs = _dogRepository.CurrentTs;
            DateTime calculatedTsDate = new DateTime(1970, 1, 1, 0, 0,0, 0).AddSeconds(calculatedTs);
            Assert.InRange(addedDog.LastUpdatedTimeUtc, calculatedTsDate.AddSeconds(-1), calculatedTsDate.AddSeconds(1));
            Assert.InRange(addedDog.LastUpdatedTimeRaw, calculatedTs - 1, calculatedTs + 1);
        }

        [Fact]
        public async Task UpdateAsync_PropertiesToPatch_WhenEtagMatches_UpdatesValues()
        {
            //Arrange
            string originalEtag = Guid.NewGuid().ToString();
            Person person = new("steve")
            {
                Etag = originalEtag
            };
            _personRepository.Items.TryAddAsJson(person.Id, person);

            //Act
            await _personRepository.UpdateAsync(person.Id, builder => builder.Replace(p => p.Name, "kenny"), default, default, originalEtag);

            //Assert
            Person updatedPerson = _personRepository.DeserializeItem(_personRepository.Items.First().Value);
            Assert.Equal("kenny", updatedPerson.Name);
            Assert.Equal(person.Id, updatedPerson.Id);
            Assert.True(!string.IsNullOrWhiteSpace(updatedPerson.Etag) && updatedPerson.Etag != Guid.Empty.ToString() && updatedPerson.Etag != originalEtag);
        }

        [Fact]
        public async Task UpdateAsync_PropertiesToPatch_WhenEtagIsNull_UpdatesValues()
        {
            //Arrange
            string originalEtag = Guid.NewGuid().ToString();
            Person person = new("steve")
            {
                Etag = originalEtag
            };
            _personRepository.Items.TryAddAsJson(person.Id, person);

            //Act
            await _personRepository.UpdateAsync(person.Id, builder => builder.Replace(p => p.Name, "kenny"), default, default, null);

            //Assert
            Person updatedPerson = _personRepository.DeserializeItem(_personRepository.Items.First().Value);
            Assert.Equal("kenny", updatedPerson.Name);
            Assert.Equal(person.Id, updatedPerson.Id);
            Assert.True(!string.IsNullOrWhiteSpace(updatedPerson.Etag) && updatedPerson.Etag != Guid.Empty.ToString() && updatedPerson.Etag != originalEtag);
        }


        [Fact]
        public async Task UpdateAsync_PropertiesToPatch_WhenEtagIsEmpty_UpdatesValues()
        {
            //Arrange
            string originalEtag = Guid.NewGuid().ToString();
            Person person = new("steve")
            {
                Etag = originalEtag
            };
            _personRepository.Items.TryAddAsJson(person.Id, person);

            //Act
            await _personRepository.UpdateAsync(person.Id, builder => builder.Replace(p => p.Name, "kenny"), default, default, string.Empty);

            //Assert
            Person updatedPerson = _personRepository.DeserializeItem(_personRepository.Items.First().Value);
            Assert.Equal("kenny", updatedPerson.Name);
            Assert.Equal(person.Id, updatedPerson.Id);
            Assert.True(!string.IsNullOrWhiteSpace(updatedPerson.Etag) && updatedPerson.Etag != Guid.Empty.ToString() && updatedPerson.Etag != originalEtag);
        }

        [Fact]
        public async Task UpdateAsync_PropertiesToPatch_WhenEtagIsWhiteSpace_UpdatesValues()
        {
            //Arrange
            string originalEtag = Guid.NewGuid().ToString();
            Person person = new("steve")
            {
                Etag = originalEtag
            };
            _personRepository.Items.TryAddAsJson(person.Id, person);

            //Act
            await _personRepository.UpdateAsync(person.Id, builder => builder.Replace(p => p.Name, "kenny"), default, default, "            ");

            //Assert
            Person updatedPerson = _personRepository.DeserializeItem(_personRepository.Items.First().Value);
            Assert.Equal("kenny", updatedPerson.Name);
            Assert.Equal(person.Id, updatedPerson.Id);
            Assert.True(!string.IsNullOrWhiteSpace(updatedPerson.Etag) && updatedPerson.Etag != Guid.Empty.ToString() && updatedPerson.Etag != originalEtag);
        }

        [Fact]
        public async Task UpdateAsync_PropertiesToPatch_WhenEtagDoesNotMatch_ThrowsCosmosException()
        {
            //Arrange
            string originalEtag = Guid.NewGuid().ToString();
            Person person = new("steve")
            {
                Etag = originalEtag
            };
            _personRepository.Items.TryAddAsJson(person.Id, person);

            //Act & Assert
            CosmosException cosmosException = await Assert.ThrowsAsync<CosmosException>(() => _personRepository.UpdateAsync(person.Id, builder => builder.Replace(p => p.Name, "kenny"), default, default, Guid.NewGuid().ToString()).AsTask());
            Assert.Equal(HttpStatusCode.PreconditionFailed, cosmosException.StatusCode);


            Person updatedPerson = _personRepository.DeserializeItem(_personRepository.Items.First().Value);
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

            _rootObjectRepository.Items.TryAddAsJson(root.Id, root);

            //Act
            await _rootObjectRepository.UpdateAsync(root.Id, builder =>
                builder.Replace(x => x.Type1, "CBA")
                    .Replace(x => x.NestedObject, new NestedObject
                    {
                        Property1 = "prop2",
                        Property2 = 2
                    }));

            //Assert
            RootObject deserialisedItem = _rootObjectRepository.DeserializeItem(_rootObjectRepository.Items.First().Value);
            Assert.Equal("CBA", deserialisedItem.Type1);
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
            _dogRepository.Items.TryAddAsJson(dog1.Id, dog1);
            _dogRepository.Items.TryAddAsJson(dog2.Id, dog2);
            _dogRepository.Items.TryAddAsJson(dog3.Id, dog3);
            int pageSize = 2;
            int pageNumber = 2;

            //Act
            IPageExtended<Dog> dogs = await _dogRepository.PageAsync(d => d.Breed == "golden retriever", pageNumber, pageSize);

            //Assert
            List<Dog> enumerable = dogs.Items.ToList();
            Assert.False(enumerable.Any());
        }

        [Fact]
        public async Task PageAsync_PredicateThatDoesMatch_ReturnsItemInList()
        {
            //Arrange
            Dog dog1 = new("cocker spaniel");
            Dog dog2 = new("cocker spaniel");
            Dog dog3 = new("cocker spaniel");
            Dog dog4 = new("cocker spaniel");
            Dog dog5 = new("cocker spaniel");
            Dog dog6 = new("cocker spaniel");
            Dog dog7 = new("golden retriever");
            _dogRepository.Items.TryAddAsJson(dog1.Id, dog1);
            _dogRepository.Items.TryAddAsJson(dog2.Id, dog2);
            _dogRepository.Items.TryAddAsJson(dog3.Id, dog3);
            _dogRepository.Items.TryAddAsJson(dog4.Id, dog4);
            _dogRepository.Items.TryAddAsJson(dog5.Id, dog5);
            _dogRepository.Items.TryAddAsJson(dog6.Id, dog6);
            _dogRepository.Items.TryAddAsJson(dog7.Id, dog7);
            int pageSize = 2;
            int pageNumber = 2;

            List<Dog> expectedList = _dogRepository.Items.Select(d => JsonConvert.DeserializeObject<Dog>(d.Value))
                                                         .Where(d => d.Breed == "cocker spaniel")
                                                         .Skip(pageSize * (pageNumber - 1))
                                                         .Take(pageSize)
                                                         .ToList();

            //Act
            IPageExtended<Dog> dogs = await _dogRepository.PageAsync(d => d.Breed == "cocker spaniel", pageNumber, pageSize);

            //Assert
            List<Dog> enumerable = dogs.Items.ToList();
            Assert.True(enumerable.Any());
            Assert.Equal(expectedList, enumerable, new DogComparer());
            Assert.True(dogs.HasPreviousPage);
            Assert.Equal(1, dogs.PreviousPageNumber);
            Assert.True(dogs.HasNextPage);
            Assert.Equal(3, dogs.NextPageNumber);
            Assert.Equal(3, dogs.TotalPages);
        }
    }
}