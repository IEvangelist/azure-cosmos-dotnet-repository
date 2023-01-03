// Copyright (c) David Pine. All rights reserved.
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

/// <summary>
/// Required as item options are cached in a static collection, the key is the Type
/// </summary>
public class DisabledOffer : Offer
{
    public DisabledOffer(string offerType, string createdBy) : base(offerType, createdBy)
    {
    }
}

[Trait("Category", "Acceptance")]
[Trait("Type", "Container")]
public class DisabledStrictTypeCheckingTests : CosmosRepositoryAcceptanceTest
{
    private const string OffersDatabaseName = "offers";
    private const string OffersContainerName = "offers";
    private const string OffersPartitionKeyPath = "/partitionKey";

    private static readonly Action<RepositoryOptions> DisabledStrictTypeCheckingOptions = options =>
    {
        options.DatabaseId = BuildDatabaseName(OffersDatabaseName);
        options.CosmosConnectionString = GetCosmosConnectionString();
        options.ContainerPerItemType = true;

        options.ContainerBuilder.Configure<DisabledOffer>(builder =>
        {
            builder.WithContainer(OffersContainerName);
            builder.WithPartitionKey(OffersPartitionKeyPath);
            builder.WithContainerDefaultTimeToLive(TimeSpan.FromMinutes(10));
            builder.WithoutStrictTypeChecking();
        });

        options.ContainerBuilder.Configure<BuyOneGetOneFreeOffer>(builder =>
        {
            builder.WithContainer(OffersContainerName);
            builder.WithPartitionKey(OffersPartitionKeyPath);
            builder.WithContainerDefaultTimeToLive(TimeSpan.FromMinutes(10));
        });

        options.ContainerBuilder.Configure<DiscountOf20Percent>(builder =>
        {
            builder.WithContainer(OffersContainerName);
            builder.WithPartitionKey(OffersPartitionKeyPath);
            builder.WithContainerDefaultTimeToLive(TimeSpan.FromMinutes(10));
        });
    };

    [Fact]
    public async Task GetAsync_BaseClassSetupWithoutStrictTypeChecking_ReturnSubClassesOnlyDeserializedIntoBaseType()
    {
        try
        {
            //Arrange
            await GetClient().UseClientAsync(PruneDatabases);
            IRepository<DisabledOffer> offersRepository =
                _provider.GetRequiredService<IRepository<DisabledOffer>>();
            IRepository<BuyOneGetOneFreeOffer> buyOneGetOneFreeOffersRepository =
                _provider.GetRequiredService<IRepository<BuyOneGetOneFreeOffer>>();
            IRepository<DiscountOf20Percent> discountOf20PercentRepository =
                _provider.GetRequiredService<IRepository<DiscountOf20Percent>>();

            DiscountOf20Percent discountOf20Percent = new("bob");
            BuyOneGetOneFreeOffer buyOneGetOneFreeOffer = new("fred");

            await buyOneGetOneFreeOffersRepository.CreateAsync(buyOneGetOneFreeOffer);
            await discountOf20PercentRepository.CreateAsync(discountOf20Percent);

            //Act
            IEnumerable<Offer> results = await offersRepository.GetAsync(x =>
                x.PartitionKey == nameof(BuyOneGetOneFreeOffer) ||
                x.PartitionKey == nameof(DiscountOf20Percent));

            Offer buyOneGetOneFreeOfferBase = await offersRepository.GetAsync(buyOneGetOneFreeOffer.Id, nameof(BuyOneGetOneFreeOffer));
            Offer discountOf20PercentOffOfferBase = await offersRepository.GetAsync(discountOf20Percent.Id, nameof(DiscountOf20Percent));

            //Assert
            buyOneGetOneFreeOfferBase.Should().NotBeNull();
            discountOf20PercentOffOfferBase.Should().NotBeNull();
            results.Count().Should().Be(2);
        }
        finally
        {
            await GetClient().UseClientAsync(PruneDatabases);
        }
    }

    public DisabledStrictTypeCheckingTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper, DisabledStrictTypeCheckingOptions)
    {
    }
}