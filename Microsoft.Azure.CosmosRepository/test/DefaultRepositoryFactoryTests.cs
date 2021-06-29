using System;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace Microsoft.Azure.CosmosRepositoryTests
{
    public class DefaultRepositoryFactoryTests
    {
        [Fact]
        public void NewRepositoryFactoryThrowsWithNullServiceProvider() =>
            Assert.Throws<ArgumentNullException>(() =>
                new DefaultRepositoryFactory(null));

        [Fact]
        public void RepositoryFactoryCorrectlyGetsRepositoryTest()
        {
            IConfigurationRoot configuration =
                new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["RepositoryOptions:CosmosConnectionString"] = "Testing"
                    })
                    .Build();
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);
            
            services.AddCosmosRepository();

            IServiceProvider provider = services.BuildServiceProvider();
            IRepositoryFactory factory = provider.GetRequiredService<IRepositoryFactory>();

            Assert.NotNull(factory.RepositoryOf<AnotherTestItem>());
            Assert.NotNull(factory.RepositoryOf<AndAnotherItem>());
            Assert.NotNull(factory.RepositoryOf<AndACustomEntity>());
        }
    }

    public class AnotherTestItem : Item { }
    public class AndAnotherItem : Item { }
    public class AndACustomEntity : CustomEntityBase { }

    /// <summary>
    /// Sample custom base object that implements IItem
    /// </summary>
    public abstract class CustomEntityBase : IItem
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("quest")]
        public string Quest { get; set; }

        [JsonProperty("favoritecolor")]
        public string FavoriteColor { get; set; }

        PartitionKey IItem.PartitionKey => new PartitionKey(GetPartitionKeyValue());

        public CustomEntityBase() => Type = GetType().Name;

        protected virtual string GetPartitionKeyValue() => Id;
    }
}
