// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryAcceptanceTests;

[Trait("Category", "Acceptance")]
[Trait("Type", "Container")]
public class ContainerCreationTests(ITestOutputHelper testOutputHelper) : CosmosRepositoryAcceptanceTest(testOutputHelper, UniqueKeyOptionsBuilder)
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

    [Fact(Skip = "In discussing this with Bill, we've decided that this might not be reliable enough to justify having it be a release gate.")]
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