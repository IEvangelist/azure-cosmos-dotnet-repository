namespace Microsoft.Azure.CosmosRepositoryTests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Azure.CosmosRepository;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class AndAnotherItem : Item { }

    public class AnotherTestItem : Item { }

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
}
