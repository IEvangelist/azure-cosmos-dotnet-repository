// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Extensions;

/// <summary>
/// Some useful extensions to pull values that will ve populated into <see cref="RepositoryOptions"/> from configuration
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Gets the connection string value from configuration.
    /// </summary>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance to read teh config from.</param>
    public static string? GetCosmosRepositoryConnectionString(this IConfiguration configuration) =>
        configuration.GetValue<string>(
            RepositoryOptions.CosmosConnectionStringConfigKey);

    /// <summary>
    /// Gets the database ID value from configuration.
    /// </summary>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance to read teh config from.</param>
    public static string? GetCosmosRepositoryDatabaseId(this IConfiguration configuration) =>
        configuration.GetValue<string>(RepositoryOptions.DatabaseIdConfigKey);
}