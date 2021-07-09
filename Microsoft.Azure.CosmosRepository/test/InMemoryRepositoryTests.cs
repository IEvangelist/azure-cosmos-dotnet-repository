// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
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
            _personRepository.Documents.Add(item);

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
            _dogRepository.Documents.Add(item);

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
            _dogRepository.Documents.Add(item);

            //Act
            Dog dog = await _dogRepository.GetAsync(item.Id, new PartitionKey(item.Breed));

            //Assert
            Assert.Equal(item.Id, dog.Id);
            Assert.Equal(item.Type, dog.Type);
            Assert.Equal(item.Breed, dog.Breed);
        }


    }
}