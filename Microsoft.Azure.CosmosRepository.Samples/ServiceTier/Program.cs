// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ServiceTier
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();
            await host.StartAsync();

            IRepository<Person> repository = host.Services.GetService<IRepository<Person>>();

            Person maryShaw = new Person
            {
                FirstName = "Mary",
                LastName = "Shaw",
                BirthDate = new DateTime(1972, 7, 21)
            };
            Person calvinWeatherfield = new Person
            {
                FirstName = "Calvin",
                LastName = "Weatherfield",
                BirthDate = new DateTime(1983, 2, 14)
            };

            // Creating...
            Console.WriteLine("Creating...");
            _ = await repository.CreateAsync(new[] { maryShaw, calvinWeatherfield });

            // Reading...
            Person mary = await repository.GetAsync(maryShaw.Id);
            Person calvin = (await repository.GetAsync(p => p.LastName == "Weatherfield")).Single();

            Console.WriteLine($"Read: {mary}");
            Console.WriteLine($"Read: {calvin}");

            // Updating...
            Console.WriteLine("Updating...");
            mary.BirthDate = new DateTime(1973, 7, 21); // Oops, Mary was actually born in 1973
            calvin.BirthDate = new DateTime(1982, 2, 14); // And Calvin was born in 1982...

            _ = repository.UpdateAsync(mary);
            _ = repository.UpdateAsync(calvin);

            // Read again / verify updates
            IEnumerable<Person> peopleWithoutMiddleNames = await repository.GetAsync(p => p.MiddleName == null);
            foreach (Person person in peopleWithoutMiddleNames)
            {
                Console.WriteLine($"Updated: {person}");
            }

            // Deleting...
            Console.WriteLine("Deleting...");
            await Task.WhenAll(new[]
            {
                repository.DeleteAsync(mary.Id).AsTask(),
                repository.DeleteAsync(calvin.Id).AsTask()
            });
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((_, configuration) =>
                {
                    configuration.Sources.Clear();
                    configuration.AddCommandLine(args);
                })
                .ConfigureServices((context, services) =>
                    services.AddCosmosRepository(context.Configuration));
    }
}
