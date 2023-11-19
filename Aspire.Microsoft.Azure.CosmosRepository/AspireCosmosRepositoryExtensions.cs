// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using Aspire.Microsoft.Azure.CosmosRepository.Containers;
using Aspire.Microsoft.Azure.CosmosRepository.Internals.Builders;
using Aspire.Microsoft.Azure.CosmosRepository.Internals.Containers;
using Aspire.Microsoft.Azure.CosmosRepository.Internals.Items.Configuration;
using Aspire.Microsoft.Azure.CosmosRepository.Internals.Repository;
using Aspire.Microsoft.Azure.CosmosRepository.Items.Configuration;
using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aspire.Microsoft.Azure.CosmosRepository;

public static class AspireCosmosRepositoryExtensions
{
    private const string DefaultConfigSectionName = "Aspire:Microsoft:Azure:CosmosRepository";

    public static ICosmosRepositoryBuilder AddAzureCosmosRepository(
        this IHostApplicationBuilder builder,
        string connectionName,
        Action<AzureCosmosDbSettings>? configureCoreSettings = null,
        Action<AzureCosmosDbRepositorySettings>? configureRepositorySettings = null,
        Action<CosmosClientOptions>? configureClientOptions = null)
    {
        AddAzureCosmosRepository(
            builder,
            DefaultConfigSectionName,
            configureCoreSettings,
            configureRepositorySettings,
            configureClientOptions,
            connectionName,
            serviceKey: null);

        return new CosmosRepositoryBuilder(builder);
    }

    private static IHostApplicationBuilder AddAzureCosmosRepository(
        this IHostApplicationBuilder builder,
        string configurationSectionName,
        Action<AzureCosmosDbSettings>? configureCoreSettings,
        Action<AzureCosmosDbRepositorySettings>? configureRepositorySettings,
        Action<CosmosClientOptions>? configureClientOptions,
        string connectionName,
        string? serviceKey)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services
            .AddOptions<AzureCosmosDbRepositorySettings>()
            .Configure<IConfiguration>(
                (settings, configuration) =>
                    configuration.GetSection(configurationSectionName).Bind(settings));

        builder.Services.AddSingleton(
            typeof(ICosmosItemConfiguration<>),
            typeof(DefaultCosmosItemConfiguration<>));

        builder.Services.AddSingleton<IContainerCache, DefaultContainerCache>();
        builder.Services.AddSingleton<IItemConfiguration, DefaultItemConfiguration>();
        builder.Services.AddSingleton<IRepository, DefaultRepository>();
        builder.Services.AddSingleton<IReadonlyRepository>(sp => sp.GetRequiredService<IRepository>());
        builder.Services.AddSingleton<IWriteOnlyRepository>(sp => sp.GetRequiredService<IRepository>());

        builder.AddCosmosClient(
            configurationSectionName,
            configureCoreSettings,
            configureClientOptions,
            connectionName,
            serviceKey);

        if (configureRepositorySettings is not null)
        {
            builder.Services.PostConfigure(configureRepositorySettings);
        }

        return builder;
    }

    private static void AddCosmosClient(
        this IHostApplicationBuilder builder,
        string configurationSectionName,
        Action<AzureCosmosDbSettings>? configureCoreSettings,
        Action<CosmosClientOptions>? configureClientOptions,
        string connectionName,
        string? serviceKey)
    {
        var cosmosSettings = new AzureCosmosDbSettings();
        builder.Configuration.GetSection(configurationSectionName).Bind(cosmosSettings);

        if (builder.Configuration.GetConnectionString(connectionName) is { } connectionString)
        {
            if (Uri.TryCreate(
                    connectionString,
                    UriKind.Absolute,
                    out Uri? uri))
            {
                cosmosSettings.AccountEndpoint = uri;
            }
            else
            {
                cosmosSettings.ConnectionString = connectionString;
            }
        }

        configureCoreSettings?.Invoke(cosmosSettings);

        var clientOptions = new CosmosClientOptions
        {
            CosmosClientTelemetryOptions =
            {
                // Needs to be enabled for either logging or tracing to work.
                DisableDistributedTracing = false
            }
        };

        if (cosmosSettings.Tracing)
        {
            builder.Services.AddOpenTelemetry().WithTracing(
                tracerProviderBuilder => { tracerProviderBuilder.AddSource("Azure.Cosmos.Operation"); });
        }

        configureClientOptions?.Invoke(clientOptions);

        if (serviceKey is null)
        {
            builder.Services.AddSingleton(
                sp => ConfigureClient(
                    sp,
                    cosmosSettings,
                    clientOptions,
                    connectionName,
                    configurationSectionName));
        }
        else
        {
            builder.Services.AddKeyedSingleton(
                serviceKey,
                (
                    sp,
                    _) => ConfigureClient(
                    sp,
                    cosmosSettings,
                    clientOptions,
                    connectionName,
                    configurationSectionName));
        }
    }

    private static CosmosClient ConfigureClient(
        IServiceProvider serviceProvider,
        AzureCosmosDbSettings settings,
        CosmosClientOptions clientOptions,
        string connectionName,
        string configurationSectionName)
    {
        //TODO: figure out how to do this better, it will override the settings declared above
        clientOptions.HttpClientFactory = () => ClientFactory(serviceProvider);

        if (!string.IsNullOrEmpty(settings.ConnectionString))
        {
            return new CosmosClient(
                settings.ConnectionString,
                clientOptions);
        }

        if (settings.AccountEndpoint is not null)
        {
            var credential = settings.Credential ?? new DefaultAzureCredential();
            return new CosmosClient(
                settings.AccountEndpoint.OriginalString,
                credential,
                clientOptions);
        }

        throw new InvalidOperationException(
            $"A CosmosClient could not be configured. Ensure valid connection information was provided in 'ConnectionStrings:{connectionName}' or either " +
            $"{nameof(settings.ConnectionString)} or {nameof(settings.AccountEndpoint)} must be provided " +
            $"in the '{configurationSectionName}' configuration section.");
    }

    private static HttpClient ClientFactory(
        IServiceProvider serviceProvider)
    {
        HttpClient client =
            serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient();

        var version =
            Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion
            ?? "0.0";

        client.DefaultRequestHeaders
            .UserAgent
            .ParseAdd($"ievangelist-aspire-cosmos-repository-sdk/{version}");

        return client;
    }
}