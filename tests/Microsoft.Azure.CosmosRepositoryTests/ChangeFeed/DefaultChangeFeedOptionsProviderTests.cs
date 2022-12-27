// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.Azure.CosmosRepository.ChangeFeed;
using Microsoft.Azure.CosmosRepository.ChangeFeed.Providers;
using Microsoft.Azure.CosmosRepository.Exceptions;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepositoryTests.Stubs;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Microsoft.Azure.CosmosRepositoryTests.ChangeFeed;

public class DefaultChangeFeedOptionsProviderTests
{
    private readonly RepositoryOptions _repositoryOptions = new();

    private IChangeFeedOptionsProvider CreateSut()
    {
        Mock<IOptionsMonitor<RepositoryOptions>> monitor = new();
        monitor.SetupGet(o => o.CurrentValue)
            .Returns(_repositoryOptions);

        return new DefaultChangeFeedOptionsProvider(monitor.Object);
    }

    [Fact]
    public void GetOptionsForItems_ItemTypes_GetsChangeFeedOptions()
    {
        //Arrange
        IChangeFeedOptionsProvider sut = CreateSut();

        _repositoryOptions.ContainerBuilder.Configure<TestItem>(builder =>
        {
            builder.WithChangeFeedMonitoring();
        });

        _repositoryOptions.ContainerBuilder.Configure<AnotherTestItem>(builder =>
        {
            builder.WithChangeFeedMonitoring();
        });

        //Act
        ChangeFeedOptions options = sut.GetOptionsForItems(new[] { typeof(TestItem), typeof(AnotherTestItem) });

        //Assert
        Assert.Equal("default", options.InstanceName);
    }

    [Fact]
    public void GetOptionsForItems_ItemTypesWithDifferentChangeFeedOptions_ThrowsMissMatchedChangeFeedOptionsException()
    {
        //Arrange
        IChangeFeedOptionsProvider sut = CreateSut();

        _repositoryOptions.ContainerBuilder.Configure<TestItem>(builder =>
        {
            builder.WithChangeFeedMonitoring();
        });

        _repositoryOptions.ContainerBuilder.Configure<AnotherTestItem>(builder =>
        {
            builder.WithChangeFeedMonitoring(options =>
            {
                options.InstanceName = "different";
            });
        });

        IReadOnlyList<Type> types = new[] { typeof(TestItem), typeof(AnotherTestItem) };

        //Act
        //Assert
        Assert.Throws<MissMatchedChangeFeedOptionsException>(() =>
            sut.GetOptionsForItems(types));
    }
}