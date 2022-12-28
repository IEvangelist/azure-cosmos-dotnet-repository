// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Cosmos;

Console.WriteLine("Starting to query all databases from cosmos account");

using CosmosClient client = new(
    Environment.GetEnvironmentVariable("CosmosConnectionString"));

using FeedIterator<DatabaseProperties> databases =
    client.GetDatabaseQueryIterator<DatabaseProperties>("SELECT * FROM c");

var total = 0;

while (databases.HasMoreResults)
{
    FeedResponse<DatabaseProperties> response = await databases.ReadNextAsync();
    foreach (DatabaseProperties databaseProperties in response)
    {
        Console.WriteLine($"Deleting database {databaseProperties.Id}");

        Database database = client.GetDatabase(databaseProperties.Id);

        await database.DeleteAsync();

        Console.WriteLine($"Deleted database {database.Id}");
        total++;
    }
}

Console.WriteLine($"Deleted a total of ({total} databases");