// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository;
using Xunit;

namespace Microsoft.Azure.CosmosRepositoryTests
{
    class Person : Item
    {
        public string Name { get; }

        public Person(string name)
        {
            Name = name;
        }
    }


    class Dog : Item
    {
        public string Breed { get; }

        protected override string GetPartitionKeyValue() => Breed;

        public Dog(string breed)
        {
            Breed = breed;
        }
    }

    public class InMemoryRepositoryTests
    {
        private readonly InMemoryRepository<Person> _personRepository;
        private readonly InMemoryRepository<Dog> _dogRepository;

        public InMemoryRepositoryTests()
        {
            _personRepository = new InMemoryRepository<Person>();
            _dogRepository = new InMemoryRepository<Dog>();
        }

        [Fact]
        public async Task GetAsync_IdThatDoesNotExist_ThrowsCosmosException()
        {
            //Arrange
            //Act
            //Assert
            CosmosException ex = await Assert.ThrowsAsync<CosmosException>(() => _personRepository.GetAsync(Guid.NewGuid().ToString()).AsTask());

            Assert.Equal(HttpStatusCode.NotFound, ex.StatusCode);
        }

        [Fact]
        public async Task GetAsync_IdAndPartitionKeyStringNotFound_ThrowsCosmosException()
        {
            //Arrange
            //Act
            //Assert
            CosmosException ex = await Assert.ThrowsAsync<CosmosException>(() => _dogRepository.GetAsync(Guid.NewGuid().ToString(), "cocker-spaniel").AsTask());

            Assert.Equal(HttpStatusCode.NotFound, ex.StatusCode);
        }

        [Fact]
        public async Task GetAsync_IdAndPartitionKeyObjectNotFound_ThrowsCosmosException()
        {
            //Arrange
            //Act
            //Assert
            CosmosException ex = await Assert.ThrowsAsync<CosmosException>(() => _dogRepository.GetAsync(Guid.NewGuid().ToString(), new PartitionKey("cocker-spaniel")).AsTask());

            Assert.Equal(HttpStatusCode.NotFound, ex.StatusCode);
        }

        [Fact]
        public async Task GetAsync_IdThatExists_GetsItem()
        {
            //Arrange
            Person item = new("joe") {Id = Guid.NewGuid().ToString(), Type = nameof(Person)};
            _personRepository.Items.TryAdd(item.Id, item);

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
            Dog item = new("cocker-spanel") {Id = Guid.NewGuid().ToString(), Type = nameof(Dog)};
            _dogRepository.Items.TryAdd(item.Id, item);

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
            Dog item = new("cocker-spanel") {Id = Guid.NewGuid().ToString(), Type = nameof(Dog)};
            _dogRepository.Items.TryAdd(item.Id, item);

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
            _personRepository.Items.TryAdd(person.Id, person);

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
            _personRepository.Items.TryAdd(person.Id, person);

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
        public async Task CreateAsync_ItemWhereIdAlreadyExists_ThrowsCosmosException()
        {
            //Arrange
            Person item = new("joe"){Id = Guid.NewGuid().ToString(), Type = nameof(Person)};
            _personRepository.Items.TryAdd(item.Id, item);

            //Act
            CosmosException ex = await Assert.ThrowsAsync<CosmosException>(() => _personRepository.CreateAsync(new Person("joe") {Id = item.Id, Type = nameof(Person)}).AsTask());

            Assert.Equal(HttpStatusCode.Conflict, ex.StatusCode);
        }

        [Fact]
        public async Task CreateAsync_Item_CreatesItem()
        {
            //Arrange
            Person item = new("joe"){Id = Guid.NewGuid().ToString(), Type = nameof(Person)};

            //Act
            Person person = await _personRepository.CreateAsync(item);

            Person addedPerson = _personRepository.Items.Values.First();

            Assert.Equal(item.Name, person.Name);
            Assert.Equal(item.Id, person.Id);
            Assert.Equal(item.Type, person.Type);

            Assert.Equal(item.Name, addedPerson.Name);
            Assert.Equal(item.Id, addedPerson.Id);
            Assert.Equal(item.Type, addedPerson.Type);
        }

        [Fact]
        public async Task CreateAsync_ManyItems_CreatesAllItems()
        {
            //Arrange
            List<Person> items = new()
            {
                new("joe") {Id = Guid.NewGuid().ToString(), Type = nameof(Person)},
                new("bill") {Id = Guid.NewGuid().ToString(), Type = nameof(Person)},
                new("fred") {Id = Guid.NewGuid().ToString(), Type = nameof(Person)},
            };

            //Act
            IEnumerable<Person> people = (await _personRepository.CreateAsync(items)).ToList();

            foreach (Person item in items)
            {
                Person addedPerson = _personRepository.Items.Values.First(i => i.Id == item.Id);
                Person person = people.First(i => i.Id == item.Id);

                Assert.Equal(item.Name, person.Name);
                Assert.Equal(item.Id, person.Id);
                Assert.Equal(item.Type, person.Type);

                Assert.Equal(item.Name, addedPerson.Name);
                Assert.Equal(item.Id, addedPerson.Id);
                Assert.Equal(item.Type, addedPerson.Type);
            }
        }

        [Fact]
        public async Task CreateAsync_ManyItemsWhereOneHasIdThatAlreadyExists_CreatesInitalItemsThenThrowsCosmosException()
        {
            //Arrange
            List<Person> items = new()
            {
                new("joe") {Id = Guid.NewGuid().ToString(), Type = nameof(Person)},
                new("bill") {Id = Guid.NewGuid().ToString(), Type = nameof(Person)},
                new("fred") {Id = Guid.NewGuid().ToString(), Type = nameof(Person)},
            };

            Person badPerson = new("copy") {Id = items.First().Id, Type = nameof(Person)};
            items.Add(badPerson);

            //Act
            CosmosException ex = await Assert.ThrowsAsync<CosmosException>(() => _personRepository.CreateAsync(items).AsTask());

            Assert.Equal(HttpStatusCode.Conflict, ex.StatusCode);

            foreach (Person item in items.Where(i => i.Name != badPerson.Name))
            {
                Person person = items.First(i => i.Id == item.Id);

                Assert.Equal(item.Name, person.Name);
                Assert.Equal(item.Id, person.Id);
                Assert.Equal(item.Type, person.Type);
            }
        }


        [Fact]
        public async Task DeleteAsync_IdThatDoesNotExist_ThrowsCosmosException()
        {
            //Arrange
            //Act
            CosmosException ex = await Assert.ThrowsAsync<CosmosException>(() => _personRepository.DeleteAsync("does-not-exist").AsTask());

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
            _personRepository.Items.TryAdd(person.Id, person);

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
            _dogRepository.Items.TryAdd(dog.Id, dog);

            //Act
            await _dogRepository.DeleteAsync(dog.Id, dog.Breed);

            //Assert
            Assert.False(_dogRepository.Items.Any());
        }

        [Fact]
        public async Task UpdateAsync_ItemThatDoesNotExist_AddsItem()
        {
            //Arrange
            Person item = new("joe"){Id = Guid.NewGuid().ToString(), Type = nameof(Person)};

            //Act
            Person person = await _personRepository.UpdateAsync(item);

            Person addedPerson = _personRepository.Items.Values.First();

            Assert.Equal(item.Name, person.Name);
            Assert.Equal(item.Id, person.Id);
            Assert.Equal(item.Type, person.Type);

            Assert.Equal(item.Name, addedPerson.Name);
            Assert.Equal(item.Id, addedPerson.Id);
            Assert.Equal(item.Type, addedPerson.Type);
        }

        [Fact]
        public async Task UpdateAsync_ManyItems_UpdatesAllItems()
        {
            //Arrange
            List<Person> items = new()
            {
                new("joe") { Id = Guid.NewGuid().ToString(), Type = nameof(Person) },
                new("bill") { Id = Guid.NewGuid().ToString(), Type = nameof(Person) },
                new("fred") { Id = Guid.NewGuid().ToString(), Type = nameof(Person) },
            };

            List<Person> itemsUpdate = new();

            foreach(Person item in items)
            {
                Person itemUpdate = new($"{item.Name}Updated")
                {
                    Id = item.Id,
                    Type = item.Type
                };
                itemsUpdate.Add(itemUpdate);
            }
            
            //Act
            IEnumerable<Person> people = (await _personRepository.UpdateAsync(itemsUpdate)).ToList();

            foreach (Person item in items)
            {
                Person updatedPerson = _personRepository.Items.Values.First(i => i.Id == item.Id);
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
            }
        }

        [Fact]
        public async Task UpdateAsync_ItemThatExists_UpdatesItem()
        {
            //Arrange
            Person originalPerson = new("phil");
            _personRepository.Items.TryAdd(originalPerson.Id, originalPerson);

            Person item = new("joe"){Id = originalPerson.Id};

            //Act
            Person person = await _personRepository.UpdateAsync(item);

            Person addedPerson = _personRepository.Items.Values.First();

            Assert.Equal(item.Name, person.Name);
            Assert.Equal(originalPerson.Id, person.Id);
            Assert.Equal(item.Type, person.Type);

            Assert.Equal(item.Name, addedPerson.Name);
            Assert.Equal(originalPerson.Id, addedPerson.Id);
            Assert.Equal(item.Type, addedPerson.Type);
        }

        [Fact]
        public async Task ExistsAsync_PointReadWhenItemsExists_ReturnsTrue()
        {
            //Arrange
            Person person = new("joe");

            _personRepository.Items.TryAdd(person.Id, person);

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
            _dogRepository.Items.TryAdd(dog.Id, dog);

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

            _personRepository.Items.TryAdd(person.Id, person);

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

            _dogRepository.Items.TryAdd(dog1.Id, dog1);
            _dogRepository.Items.TryAdd(dog2.Id, dog2);
            _dogRepository.Items.TryAdd(dog3.Id, dog3);

            //Act
            bool exists = await _dogRepository.ExistsAsync(d => d.Breed == "cocker spaniel" || d.Id == dog3.Id);

            //Assert
            Assert.True(exists);
        }


    }
}