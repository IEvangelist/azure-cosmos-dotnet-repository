// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using InMemoryWebTier;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
    .Build();

await host.RunAsync();
