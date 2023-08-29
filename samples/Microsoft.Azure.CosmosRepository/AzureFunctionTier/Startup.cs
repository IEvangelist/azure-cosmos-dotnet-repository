// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

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
