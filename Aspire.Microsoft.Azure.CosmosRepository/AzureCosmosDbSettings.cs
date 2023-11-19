// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Core;

namespace Aspire.Microsoft.Azure.CosmosRepository;

public class AzureCosmosDbSettings
{
    /// <summary>
    /// Gets or sets the connection string of the Azure Cosmos database to connect to.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// <para>Gets or sets a boolean value that indicates whether the OpenTelemetry tracing is enabled or not.</para>
    /// <para>Enabled by default.</para>
    /// </summary>
    public bool Tracing { get; set; } = true;

    /// <summary>
    /// A <see cref="Uri"/> referencing the Azure Cosmos DB Endpoint.
    /// This is likely to be similar to "https://{account_name}.queue.core.windows.net".
    /// </summary>
    /// <remarks>
    /// Must not contain shared access signature.
    /// Used along with <see cref="Credential"/> to establish the connection.
    /// </remarks>
    public Uri? AccountEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the credential used to authenticate to the Azure Cosmos DB endpoint.
    /// </summary>
    public TokenCredential? Credential { get; set; }

    public JsonSerializerOptions JsonSerializerOptions { get; set; } = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}