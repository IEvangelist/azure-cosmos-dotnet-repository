// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Azure.CosmosRepositoryAcceptanceTests.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Azure.CosmosRepositoryAcceptanceTests;


[Trait("Type", "Container")]
public class ContainerCreationTests : CosmosRepositoryAcceptanceTest
{

    private static readonly string UniquePolicyDb = BuildDatabaseName("unique-key-tests");
    private static readonly string UniqueKeyPolicyContainerName = "unique-key";

    private static void UniqueKeyOptionsBuilder(RepositoryOptions options)
    {
        options.CosmosConnectionString = GetCosmosConnectionString();
        options.DatabaseId = UniquePolicyDb;
        options.ContainerPerItemType = true;
        options.ContainerBuilder.Configure<UniqueKeyPolicyItem>(builder =>
        {
            builder.WithContainer(UniqueKeyPolicyContainerName);
        });
    }


    public ContainerCreationTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper, UniqueKeyOptionsBuilder)
    {
    }

    [Fact]
    public async Task Startup_ContainerWithUniqueKeyPolicy_CreatesContainerCorrectly()
    {
        //Arrange
        IRepository<UniqueKeyPolicyItem> uniqueKeyItemRepository = _provider.GetRequiredService<IRepository<UniqueKeyPolicyItem>>();
        ICosmosClientProvider clientProvider = _provider.GetRequiredService<ICosmosClientProvider>();

        Database? database = null;

        try
        {
            //Act
            database = await clientProvider.UseClientAsync(x => Task.FromResult(x.GetDatabase(UniquePolicyDb)));

            UniqueKeyPolicyItem uniqueItem1 = new("bob", 10);
            UniqueKeyPolicyItem uniqueItem2 = new("gil", 11);
            UniqueKeyPolicyItem uniqueItem3 = new("bob", 11);
            UniqueKeyPolicyItem dupeItem = new("bob", 10);

            await uniqueKeyItemRepository.CreateAsync(uniqueItem1);
            await uniqueKeyItemRepository.CreateAsync(uniqueItem2);
            await uniqueKeyItemRepository.CreateAsync(uniqueItem3);

            //Assert

            ContainerProperties? properties = await GetContainerProperties(database, UniqueKeyPolicyContainerName);
            properties.Should().NotBeNull();
            properties!.UniqueKeyPolicy.UniqueKeys.Count.Should().Be(1);
            UniqueKey keys = properties.UniqueKeyPolicy.UniqueKeys.First();
            keys.Paths.Should().Contain("/firstName");
            keys.Paths.Should().Contain("/age");

            await Assert.ThrowsAsync<CosmosException>(() => uniqueKeyItemRepository.CreateAsync(dupeItem).AsTask());
        }
        finally
        {
            if (database != null)
            {
                await database.DeleteAsync();
            }
        }
    }
}