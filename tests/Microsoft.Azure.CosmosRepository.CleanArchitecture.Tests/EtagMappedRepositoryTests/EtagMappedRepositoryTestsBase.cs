// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Bogus;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Extensions.Options;
using Moq;
using Moq.AutoMock;

namespace Microsoft.Azure.CosmosRepository.CleanArchitecture.Tests.EtagMappedRepositoryTests;

internal abstract class EtagMappedRepositoryTestsBase
{
    protected AutoMocker _mocker;
    protected Faker<TestTypes.PersonItem> _personItemFaker;

    protected readonly RepositoryOptions DefaultRepositoryOptions = new ()
    {
        OptimizeBandwidth = false
    };

    protected Mock<IRepository<TestTypes.PersonItem>> PersonItemRepositoryMock =>
        _mocker.GetMock<IRepository<TestTypes.PersonItem>>();

    protected Mock<IMapper<TestTypes.PersonItem, TestTypes.PersonEntity>> PersonMapperMock =>
        _mocker.GetMock<IMapper<TestTypes.PersonItem, TestTypes.PersonEntity>>();

    protected Mock<IEtagCache> EtagCacheMock =>
        _mocker.GetMock<IEtagCache>();

    protected Mock<IOptionsMonitor<RepositoryOptions>> RepositoryOptionsMonitorMock =>
        _mocker.GetMock<IOptionsMonitor<RepositoryOptions>>();

    protected IEtagMappedRepository<TestTypes.PersonItem, TestTypes.PersonEntity> CreateSut() =>
        _mocker.CreateInstance<EtagMappedRepository<TestTypes.PersonItem, TestTypes.PersonEntity>>();


    protected IEnumerable<
            (TestTypes.PersonItem personItem,
            TestTypes.PersonEntity personEntity)>
        CreatePeople(int count = 5)
    {
        for (int i = 0; i < count; i++)
        {
            yield return CreatePerson();
        }
    }

    protected (TestTypes.PersonItem personItem, TestTypes.PersonEntity personEntity) CreatePerson(string? etag = null)
    {
        TestTypes.PersonItem personItem = _personItemFaker.Generate();
        personItem.Etag = etag ?? Guid.NewGuid().ToString();

        return (
            personItem,
            new TestTypes.PersonEntity(personItem.Name, personItem.Age, personItem.Id)
        );
    }
}