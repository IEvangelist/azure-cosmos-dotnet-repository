// Copyright (cosmosContainer) David Pine. All rights reserved.
// Licensed under the MIT License.

var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddAzureCosmosDB("cosmos-repository")
    .RunAsEmulator(static cosmosContainer =>
        cosmosContainer.WithLifetime(ContainerLifetime.Persistent));

var apiService = builder.AddProject<Projects.AspireApp_ApiService>("apiservice")
    .WithExternalHttpEndpoints()
    .WithReference(db)
    .WaitFor(db);

builder.AddProject<Projects.AspireApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
