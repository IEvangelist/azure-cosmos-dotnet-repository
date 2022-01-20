// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepositoryAcceptanceTests.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Azure.CosmosRepositoryAcceptanceTests;

public class TypeCheckingTests : CosmosRepositoryAcceptanceTest
{
    private const string OffersContainerName = "offers";
    private const string OffersPartitionKeyPath = "/partitionKey";

    private static readonly Action<RepositoryOptions> TypeCheckingTestOptions = options =>
    {
        options.ContainerBuilder.Configure<Offer>(builder =>
        {
            builder.WithContainer(OffersContainerName);
            builder.WithPartitionKey(OffersPartitionKeyPath);
            builder.WithContainerDefaultTimeToLive(TimeSpan.FromMinutes(10));
        });

        options.ContainerBuilder.Configure<BuyOneGetOneFreeOffer>(builder =>
        {
            builder.WithContainer(OffersContainerName);
            builder.WithPartitionKey(OffersPartitionKeyPath);
            builder.WithContainerDefaultTimeToLive(TimeSpan.FromMinutes(10));
        });

        options.ContainerBuilder.Configure<BuyOneGetOneHalfPriceOffer>(builder =>
        {
            builder.WithContainer(OffersContainerName);
            builder.WithPartitionKey(OffersPartitionKeyPath);
            builder.WithContainerDefaultTimeToLive(TimeSpan.FromMinutes(10));
        });
    };

    [Fact]
    public async Task GetAsync_BaseClass_ShouldReturnSubClassesOnlyDeserializedIntoBaseType()
    {
        //Arrange
        IRepository<Offer> offersRepository =
            _provider.GetRequiredService<IRepository<Offer>>();
        IRepository<BuyOneGetOneFreeOffer> buyOneGetOneFreeOffersRepository =
            _provider.GetRequiredService<IRepository<BuyOneGetOneFreeOffer>>();
        IRepository<BuyOneGetOneHalfPriceOffer> buyOneGetOneHalfPriceRepository =
            _provider.GetRequiredService<IRepository<BuyOneGetOneHalfPriceOffer>>();

        BuyOneGetOneHalfPriceOffer buyOneGetOneHalfPriceOffer = new("bob");
        BuyOneGetOneFreeOffer buyOneGetOneFreeOffer = new("fred");

        await buyOneGetOneFreeOffersRepository.CreateAsync(buyOneGetOneFreeOffer);
        await buyOneGetOneHalfPriceRepository.CreateAsync(buyOneGetOneHalfPriceOffer);

        //Act
        IEnumerable<Offer> results = await offersRepository.GetAsync(x =>
            x.OfferType == nameof(BuyOneGetOneFreeOffer) &&
            x.OfferType == nameof(BuyOneGetOneHalfPriceOffer));

        //Assert
        results.Count().Should().Be(2);
    }

    public TypeCheckingTests(ITestOutputHelper testOutputHelper) : base(TypeCheckingTestOptions, testOutputHelper)
    {
    }
}