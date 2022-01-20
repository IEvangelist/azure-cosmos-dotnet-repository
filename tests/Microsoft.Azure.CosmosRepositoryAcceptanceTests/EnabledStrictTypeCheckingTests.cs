// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
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
public class EnabledOffer : Offer
{
    public EnabledOffer(string offerType, string createdBy) : base(offerType, createdBy)
    {
    }
}

[Trait("Category", "Acceptance")]
[Trait("Type", "Container")]
public class EnabledStrictTypeCheckingTests : CosmosRepositoryAcceptanceTest
{
    private const string OffersDatabaseName = "offers-db";
    private const string OffersContainerName = "offers";
    private const string OffersPartitionKeyPath = "/partitionKey";

    private static readonly Action<RepositoryOptions> EnabledStrictTypeCheckingOptions = options =>
    {
        options.DatabaseId = OffersDatabaseName;
        options.CosmosConnectionString = GetCosmosConnectionString();
        options.ContainerPerItemType = true;

        options.ContainerBuilder.Configure<EnabledOffer>(builder =>
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

        options.ContainerBuilder.Configure<DiscountOf20Percent>(builder =>
        {
            builder.WithContainer(OffersContainerName);
            builder.WithPartitionKey(OffersPartitionKeyPath);
            builder.WithContainerDefaultTimeToLive(TimeSpan.FromMinutes(10));
        });
    };

    [Fact]
    public async Task GetAsync_BaseClassSetupWithStrictTypeChecking_ReturnSubClassesOnlyDeserializedIntoBaseType()
    {
        try
        {
            //Arrange
            await GetClient().UseClientAsync(x =>
                DeleteDatabaseIfExists(OffersDatabaseName, x));
            IRepository<EnabledOffer> offersRepository =
                _provider.GetRequiredService<IRepository<EnabledOffer>>();
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

            //Assert
            Offer offer1 = await offersRepository.GetAsync(discountOf20Percent.Id, nameof(DiscountOf20Percent));
            offer1.Should().BeNull();

            Offer offer2 = await offersRepository.GetAsync(buyOneGetOneFreeOffer.Id, nameof(BuyOneGetOneFreeOffer));
            offer2.Should().BeNull();

            results.Count().Should().Be(0);
        }
        finally
        {
            await GetClient().UseClientAsync(x =>
                DeleteDatabaseIfExists(OffersDatabaseName, x));
        }
    }

    public EnabledStrictTypeCheckingTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper, EnabledStrictTypeCheckingOptions)
    {
    }
}