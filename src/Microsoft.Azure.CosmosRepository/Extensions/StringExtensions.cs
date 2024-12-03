// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Extensions;

internal static class StringExtensions
{
    /// <summary>
    /// Gets the well-known and documented Azure Cosmos DB emulator account key.
    /// See <a href="https://learn.microsoft.com/azure/cosmos-db/emulator#authentication"></a>
    /// </summary>
    private const string EmulatorAccountKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

    internal static bool IsEmulatorConnectionString(this string? connectionString)
    {
        if (connectionString is null)
        {
            return false;
        }

        var builder = new DbConnectionStringBuilder
        {
            ConnectionString = connectionString
        };

        if (!builder.TryGetValue("AccountKey", out var value))
        {
            return false;
        }

        var accountKeyFromConnectionString = value.ToString();

        return accountKeyFromConnectionString is EmulatorAccountKey;
    }
}
