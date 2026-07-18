// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.Providers;

public class DefaultCosmosClientOptionsProviderTests
{
    [Fact]
    public void ClientOptionsAllowBulkExecutionUsesFinalRepositoryOptionsFromSetupAction()
    {
        CosmosClientOptions clientOptions = GetClientOptions(
            new Dictionary<string, string?>
            {
                ["RepositoryOptions:AllowBulkExecution"] = bool.FalseString
            },
            services => services.AddCosmosRepository(options => options.AllowBulkExecution = true));

        clientOptions.AllowBulkExecution.Should().BeTrue();
    }

    [Fact]
    public void ClientOptionsAllowBulkExecutionUsesFinalRepositoryOptionsFromPostConfigure()
    {
        CosmosClientOptions clientOptions = GetClientOptions(
            new Dictionary<string, string?>
            {
                ["RepositoryOptions:AllowBulkExecution"] = bool.FalseString
            },
            services =>
            {
                services.AddCosmosRepository();
                services.PostConfigure<RepositoryOptions>(options => options.AllowBulkExecution = true);
            });

        clientOptions.AllowBulkExecution.Should().BeTrue();
    }

    [Fact]
    public void ClientOptionsSerializerOptionsUsesFinalRepositoryOptionsFromSetupAction()
    {
        CosmosClientOptions clientOptions = GetClientOptions(
            new Dictionary<string, string?>(),
            services => services.AddCosmosRepository(options => options.SerializationOptions = new RepositorySerializationOptions
            {
                IgnoreNullValues = true,
                Indented = true
            }));

        clientOptions.SerializerOptions.Should().NotBeNull();
        clientOptions.SerializerOptions!.IgnoreNullValues.Should().BeTrue();
        clientOptions.SerializerOptions.Indented.Should().BeTrue();
    }

    private static CosmosClientOptions GetClientOptions(
        Dictionary<string, string?> configurationValues,
        Action<IServiceCollection> registerServices)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationValues)
            .Build();

        IServiceCollection services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);

        registerServices(services);

        using ServiceProvider provider = services.BuildServiceProvider();

        return provider.GetRequiredService<ICosmosClientOptionsProvider>().ClientOptions;
    }
}
