using System;
using System.Collections.Generic;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            IServiceCollection services = new ServiceCollection();
            IConfigurationRoot configuration =
                new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["RepositoryOptions:CosmosConnectionString"] = "Testing"
                    })
                    .Build();
            services.AddCosmosRepository(configuration);

            IServiceProvider provider = services.BuildServiceProvider();
            IRepositoryFactory factory = provider.GetRequiredService<IRepositoryFactory>();

            Assert.NotNull(factory.RepositoryOf<AnotherTestItem>());
            Assert.NotNull(factory.RepositoryOf<AndAnotherItem>());
        }
    }

    public class AnotherTestItem : Item { }
    public class AndAnotherItem : Item { }
}