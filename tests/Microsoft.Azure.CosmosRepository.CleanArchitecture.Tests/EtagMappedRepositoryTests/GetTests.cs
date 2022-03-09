// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Extensions.Options;
using Moq;
using Moq.AutoMock;
using Moq.Language.Flow;
using NUnit.Framework;

namespace Microsoft.Azure.CosmosRepository.CleanArchitecture.Tests.EtagMappedRepositoryTests;

[TestFixture]
internal class GetTests : EtagMappedRepositoryTestsBase
{
    [SetUp]
    public void SetUp()
    {
        _mocker = new AutoMocker(MockBehavior.Loose);
        _personItemFaker = new();
        _personItemFaker
            .RuleFor(p =>
                p.Name, f => f.Name.FullName())
            .RuleFor(p =>
                p.Age, f => f.Random.Number(15, 45));
        RepositoryOptionsMonitorMock.SetupGet(x =>
            x.CurrentValue).Returns(DefaultRepositoryOptions);
    }

    [Test]
    [TestCase(null)]
    [TestCase("partitionKey")]
    public async Task TryGetAsync_IdAndPartitionKeyString_MapsAndCallsThrough(string? partitionKey)
    {
        // Arrange
        (TestTypes.PersonItem personItem, TestTypes.PersonEntity personEntity) =
            CreatePerson();

        PersonItemRepositoryMock.Setup(x =>
                x.TryGetAsync(personItem.Id, partitionKey, CancellationToken.None))
            .ReturnsAsync(personItem);

        PersonMapperMock.Setup(x =>
                x.MapAsync(personItem))
            .ReturnsAsync(personEntity);

        IEtagMappedRepository<TestTypes.PersonItem, TestTypes.PersonEntity> sut = CreateSut();

        // Act
        TestTypes.PersonEntity? getAsyncResponse = await sut.TryGetAsync(personItem.Id, partitionKey, CancellationToken.None);

        // Assert
        PersonItemRepositoryMock.Verify(x =>
                x.TryGetAsync(personItem.Id, partitionKey, CancellationToken.None),
            Times.Once);

        PersonMapperMock.Verify(x =>
                x.MapAsync(personItem),
            Times.Once);

        EtagCacheMock.Verify(x =>
                x.StoreEtag(personItem),
            Times.Once);

        getAsyncResponse.Should().BeEquivalentTo(personEntity);
    }

    [Test]
    [TestCase(null)]
    [TestCase("partitionKey")]
    public async Task GetAsync_IdAndPartitionKeyString_MapsAndCallsThrough(string? partitionKey)
    {
        // Arrange
        (TestTypes.PersonItem personItem, TestTypes.PersonEntity personEntity) =
            CreatePerson();

        PersonItemRepositoryMock.Setup(x =>
                x.GetAsync(personItem.Id, partitionKey, CancellationToken.None))
            .ReturnsAsync(personItem);

        PersonMapperMock.Setup(x =>
                x.MapAsync(personItem))
            .ReturnsAsync(personEntity);

        IEtagMappedRepository<TestTypes.PersonItem, TestTypes.PersonEntity> sut = CreateSut();

        // Act
        TestTypes.PersonEntity getAsyncResponse = await sut.GetAsync(personItem.Id, partitionKey, CancellationToken.None);

        // Assert
        PersonItemRepositoryMock.Verify(x =>
                x.GetAsync(personItem.Id, partitionKey, CancellationToken.None),
            Times.Once);

        PersonMapperMock.Verify(x =>
                x.MapAsync(personItem),
            Times.Once);

        EtagCacheMock.Verify(x =>
                x.StoreEtag(personItem),
            Times.Once);

        getAsyncResponse.Should().BeEquivalentTo(personEntity);
    }

    [Test]
    public async Task GetAsync_IdAndPartitionKey_MapsAndCallsThrough()
    {
        // Arrange
        PartitionKey partitionKey = new ("partitionKey");
        (TestTypes.PersonItem personItem, TestTypes.PersonEntity personEntity) =
            CreatePerson();

        PersonItemRepositoryMock.Setup(x =>
                x.GetAsync(personItem.Id, partitionKey, CancellationToken.None))
            .ReturnsAsync(personItem);

        PersonMapperMock.Setup(x =>
                x.MapAsync(personItem))
            .ReturnsAsync(personEntity);

        IEtagMappedRepository<TestTypes.PersonItem, TestTypes.PersonEntity> sut = CreateSut();

        // Act
        TestTypes.PersonEntity getAsyncResponse = await sut.GetAsync(personItem.Id, partitionKey, CancellationToken.None);

        // Assert
        PersonItemRepositoryMock.Verify(x =>
                x.GetAsync(personItem.Id, partitionKey, CancellationToken.None),
            Times.Once);

        PersonMapperMock.Verify(x =>
                x.MapAsync(personItem),
            Times.Once);

        EtagCacheMock.Verify(x =>
                x.StoreEtag(personItem),
            Times.Once);

        getAsyncResponse.Should().BeEquivalentTo(personEntity);
    }

    [Test]
    public async Task GetAsync_Predicate_MapsAndCallsThrough()
    {
        // Arrange
        List<(TestTypes.PersonItem personItem, TestTypes.PersonEntity personEntity)> people = CreatePeople().ToList();

        PersonItemRepositoryMock.Setup(x =>
                x.GetAsync(It.IsAny<Expression<Func<TestTypes.PersonItem, bool>>>(), CancellationToken.None))
            .ReturnsAsync(people.Select(x => x.personItem));

        foreach ((TestTypes.PersonItem personItem, TestTypes.PersonEntity personEntity) in people)
        {
            PersonMapperMock.Setup(y =>
                y.MapAsync(personItem))
            .ReturnsAsync(personEntity);
        }

        IEtagMappedRepository<TestTypes.PersonItem, TestTypes.PersonEntity> sut = CreateSut();

        Expression<Func<TestTypes.PersonItem, bool>> predicate = (person) => true;

        // Act
        IEnumerable<TestTypes.PersonEntity> getAsyncResponse = await sut.GetAsync(predicate, CancellationToken.None);

        // Assert
        PersonItemRepositoryMock.Verify(x =>
                x.GetAsync(predicate, CancellationToken.None),
            Times.Once);

        PersonMapperMock.Verify(x =>
                x.MapAsync(It.IsAny<TestTypes.PersonItem>()),
            Times.Exactly(people.Count));
        foreach ((TestTypes.PersonItem? personItem, TestTypes.PersonEntity? _) in people)
        {
            PersonMapperMock.Verify(x =>
                    x.MapAsync(personItem),
                Times.Once);

            EtagCacheMock.Verify(x =>
                    x.StoreEtag(personItem),
                Times.Once);
        }

        getAsyncResponse.Should().BeEquivalentTo(people.Select(x => x.personEntity));
    }
}