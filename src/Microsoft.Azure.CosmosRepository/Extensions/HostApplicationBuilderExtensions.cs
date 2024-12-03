// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Extension methods for <see cref="IHostApplicationBuilder"/> to add Cosmos DB repository client.
/// </summary>
public static class HostApplicationBuilderExtensions
{
    /// <summary>
    /// Adds the Cosmos DB repository client to the application builder.
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder"/> instance.</param>
    /// <param name="connectionName">The name of the connection string in the configuration.</param>
    /// <param name="configureOptions">An optional action to configure <see cref="RepositoryOptions"/>.</param>
    /// <param name="configureClientOptions">An optional action to configure <see cref="CosmosClientOptions"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="builder"/> is <see langword="null"/>.</exception>
    public static void AddCosmosRepositoryClient(
        this IHostApplicationBuilder builder,
        string connectionName,
        Action<RepositoryOptions>? configureOptions = default,
        Action<CosmosClientOptions>? configureClientOptions = default)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        var disableTracing = builder.Configuration.GetValue($"""
            {RepositoryOptions.DefaultConfigSectionName}:DisableTracing
            """, false);

        if (!disableTracing)
        {
            builder.Services
                   .AddOpenTelemetry()
                   .WithTracing(static _ => _.AddSource("Azure.Cosmos.Operation"));
        }

        builder.Services.AddCosmosRepository(
            connectionName,
            builder.Configuration,
            options =>
            {
                if (builder.Configuration.GetConnectionString(connectionName) is string connectionString)
                {
                    if (Uri.TryCreate(connectionString, UriKind.Absolute, out var uri))
                    {
                        options.AccountEndpoint = uri;
                        options.TokenCredential = new DefaultAzureCredential();
                    }
                    else
                    {
                        options.CosmosConnectionString = connectionString;

                        // Some external consumers rely on this configuration value.
                        builder.Configuration[RepositoryOptions.CosmosConnectionStringConfigKey] = connectionString;
                    }
                }

                configureOptions?.Invoke(options);
            },
            clientOptions =>
            {
                // Needs to be enabled for either logging or tracing to work.
                clientOptions.CosmosClientTelemetryOptions.DisableDistributedTracing = false;

                if (builder.Configuration.IsEmulatorConnectionString(connectionName))
                {
                    clientOptions.ConnectionMode = ConnectionMode.Gateway;
                    clientOptions.LimitToEndpoint = true;
                }

                var cosmosApplicationName = "aspire-cosmos-repository";
                if (!string.IsNullOrEmpty(clientOptions.ApplicationName))
                {
                    cosmosApplicationName = $"{cosmosApplicationName}/{clientOptions.ApplicationName}";
                }

                clientOptions.ApplicationName = cosmosApplicationName;

                configureClientOptions?.Invoke(clientOptions);
            });
    }
}
