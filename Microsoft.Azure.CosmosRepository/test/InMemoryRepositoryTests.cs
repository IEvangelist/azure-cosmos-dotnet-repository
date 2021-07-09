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
            Person item = new Person("joe") {Id = Guid.NewGuid().ToString(), Type = nameof(Person)};
            _personRepository.Items.Add(item);

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
            Dog item = new Dog("cocker-spanel") {Id = Guid.NewGuid().ToString(), Type = nameof(Dog)};
            _dogRepository.Items.Add(item);

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
            Dog item = new Dog("cocker-spanel") {Id = Guid.NewGuid().ToString(), Type = nameof(Dog)};
            _dogRepository.Items.Add(item);

            //Act
            Dog dog = await _dogRepository.GetAsync(item.Id, new PartitionKey(item.Breed));

            //Assert
            Assert.Equal(item.Id, dog.Id);
            Assert.Equal(item.Type, dog.Type);
            Assert.Equal(item.Breed, dog.Breed);
        }

        [Fact]
        public async Task CreateAsync_ItemWhereIdAlreadyExists_ThrowsCosmosException()
        {
            //Arrange
            Person item = new Person("joe"){Id = Guid.NewGuid().ToString(), Type = nameof(Person)};
            _personRepository.Items.Add(item);

            //Act
            CosmosException ex = await Assert.ThrowsAsync<CosmosException>(() => _personRepository.CreateAsync(new Person("joe") {Id = item.Id, Type = nameof(Person)}).AsTask());

            Assert.Equal(HttpStatusCode.Conflict, ex.StatusCode);
        }

        [Fact]
        public async Task CreateAsync_Item_CreatesItem()
        {
            //Arrange
            Person item = new Person("joe"){Id = Guid.NewGuid().ToString(), Type = nameof(Person)};

            //Act
            Person person = await _personRepository.CreateAsync(item);

            Person addedPerson = _personRepository.Items.First();

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
            List<Person> items = new List<Person>()
            {
                new Person("joe") {Id = Guid.NewGuid().ToString(), Type = nameof(Person)},
                new Person("bill") {Id = Guid.NewGuid().ToString(), Type = nameof(Person)},
                new Person("fred") {Id = Guid.NewGuid().ToString(), Type = nameof(Person)},
            };

            //Act
            IEnumerable<Person> people = (await _personRepository.CreateAsync(items)).ToList();

            foreach (Person item in items)
            {
                Person addedPerson = _personRepository.Items.First(i => i.Id == item.Id);
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
            List<Person> items = new List<Person>()
            {
                new Person("joe") {Id = Guid.NewGuid().ToString(), Type = nameof(Person)},
                new Person("bill") {Id = Guid.NewGuid().ToString(), Type = nameof(Person)},
                new Person("fred") {Id = Guid.NewGuid().ToString(), Type = nameof(Person)},
            };

            Person badPerson = new Person("copy") {Id = items.First().Id, Type = nameof(Person)};
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
            Person person = new Person("joe");
            _personRepository.Items.Add(person);

            //Act
            await _personRepository.DeleteAsync(person.Id);

            //Assert
            Assert.False(_personRepository.Items.Any());
        }

        [Fact]
        public async Task DeleteAsync_ItemWithIdAndPartitionKeyThatExists_DeletesItem()
        {
            //Arrange
            Dog dog = new Dog("cocker spaniel");
            _dogRepository.Items.Add(dog);

            //Act
            await _dogRepository.DeleteAsync(dog.Id, dog.Breed);

            //Assert
            Assert.False(_dogRepository.Items.Any());
        }


    }
}