// Copyright (c) IEvangelist. All rights reserved. Licensed under the MIT License.

namespace WebTier
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;

    internal class Program
    {
        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());

        private static Task Main(string[] args) => CreateHostBuilder(args).Build().RunAsync();
    }
}
