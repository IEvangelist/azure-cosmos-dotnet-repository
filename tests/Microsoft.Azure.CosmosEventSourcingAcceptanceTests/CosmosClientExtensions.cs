// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.CosmosEventSourcingAcceptanceTests;

public static class CosmosClientExtensions
{
    public static async Task<bool> DeleteDatabaseIfExistsAsync(this CosmosClient client, string databaseName, ILogger logger)
    {
        FeedIterator<DatabaseProperties> containerQueryIterator =
            client.GetDatabaseQueryIterator<DatabaseProperties>("SELECT * FROM c");

        while (containerQueryIterator.HasMoreResults)
        {
            foreach (DatabaseProperties database in await containerQueryIterator.ReadNextAsync())
            {
                if (database.Id != databaseName)
                {
                    continue;
                }

                logger.LogInformation("Deleting database {DatabaseName}", database.Id);
                await client.GetDatabase(database.Id).DeleteAsync();
                return true;
            }
        }

        return false;
    }
}