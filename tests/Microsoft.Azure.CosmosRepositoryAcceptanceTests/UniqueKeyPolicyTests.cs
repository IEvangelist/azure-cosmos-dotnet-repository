// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Linq;
using System.Net;
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

[Trait("Category", "Acceptance")]
[Trait("Type", "Container")]
public class ContainerCreationTests : CosmosRepositoryAcceptanceTest
{
    private static readonly string UniquePolicyDb = "unique-key-tests";
    private static readonly string UniqueKeyPolicyContainerName = "unique-key";

    private static void UniqueKeyOptionsBuilder(RepositoryOptions options)
    {
        options.CosmosConnectionString = GetCosmosConnectionString();
        options.DatabaseId = BuildDatabaseName(UniquePolicyDb);
        options.ContainerPerItemType = true;
        options.ContainerBuilder.Configure<UniqueKeyPolicyItem>(builder => builder
                .WithContainer(UniqueKeyPolicyContainerName)
                .WithPartitionKey("/county"));
    }

    public ContainerCreationTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper, UniqueKeyOptionsBuilder)
    {
    }

    [Fact]
    public async Task Startup_ContainerWithUniqueKeyPolicy_CreatesContainerCorrectly()
    {
        try
        {
            //Arrange
            await GetClient().UseClientAsync(PruneDatabases);
            ICosmosClientProvider clientProvider = _provider.GetRequiredService<ICosmosClientProvider>();
            IRepository<UniqueKeyPolicyItem> uniqueKeyItemRepository = _provider.GetRequiredService<IRepository<UniqueKeyPolicyItem>>();

            //Act
            Database database = await clientProvider.UseClientAsync(x => x.CreateDatabaseIfNotExistsAsync(BuildDatabaseName(UniquePolicyDb)));

            UniqueKeyPolicyItem bobInYorkshire = new("bob", 22, "Yorkshire", "Red");
            UniqueKeyPolicyItem bobInDerbyshire = new("bob", 22, "Derbyshire", "Yellow");
            UniqueKeyPolicyItem cannotHaveAnotherBobInYorkshire = new("bob", 22, "Derbyshire", "Green");
            UniqueKeyPolicyItem jeffFromYorkshireCannotLikeRedAsWell = new("jeff", 40, "Yorkshire", "Red");

            await uniqueKeyItemRepository.CreateAsync(bobInYorkshire);
            await uniqueKeyItemRepository.CreateAsync(bobInDerbyshire);

            //Assert
            ContainerProperties? properties = await GetContainerProperties(database, UniqueKeyPolicyContainerName);
            properties.Should().NotBeNull();
            properties!.UniqueKeyPolicy.UniqueKeys.Count.Should().Be(2);
            properties.UniqueKeyPolicy.UniqueKeys
                .Count(x =>
                    x.Paths.Contains("/firstName") &&
                    x.Paths.Contains("/age") &&
                    x.Paths.Count is 2)
                .Should()
                .Be(1);

            properties.UniqueKeyPolicy.UniqueKeys
                .Count(x =>
                    x.Paths.Contains("/favouriteColor") &&
                    x.Paths.Count is 1)
                .Should()
                .Be(1);

            CosmosException ex = await Assert.ThrowsAsync<CosmosException>(() =>
                uniqueKeyItemRepository.CreateAsync(cannotHaveAnotherBobInYorkshire).AsTask());
            ex.StatusCode.Should().Be(HttpStatusCode.Conflict);

            ex = await Assert.ThrowsAsync<CosmosException>(() =>
                uniqueKeyItemRepository.CreateAsync(jeffFromYorkshireCannotLikeRedAsWell).AsTask());
            ex.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }
        finally
        {
            await GetClient().UseClientAsync(PruneDatabases);
        }
    }
}