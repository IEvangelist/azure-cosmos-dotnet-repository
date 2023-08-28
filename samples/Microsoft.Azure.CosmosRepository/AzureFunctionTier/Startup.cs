// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using User = AzureFunctionTier.Model.User;

[assembly: FunctionsStartup(typeof(AzureFunctionTier.Startup))]
namespace AzureFunctionTier;

class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder) =>
        builder.Services.AddCosmosRepository(options =>
        {
            options.ContainerPerItemType = true;
            options.ContainerBuilder.Configure<User>(containerOptions => containerOptions
                .WithContainer("users")
                .WithPartitionKey("/emailAddress")
                .WithContainerDefaultTimeToLive(TimeSpan.FromMinutes(1))
                .WithManualThroughput(500)
                .WithSyncableContainerProperties()
            );
        });
}
