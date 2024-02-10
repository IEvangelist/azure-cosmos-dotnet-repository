// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Net;
using System.Net.Sockets;
using DotNet.Testcontainers.Builders;
using Testcontainers.CosmosDb;

namespace Aspire.Microsoft.Azure.CosmosRepository.Tests;

public static class DockerCosmosDatabase
{
    private static CosmosDbContainer? _containerInstance;
    public static bool IsRunning { get; private set; }
    public static string ContainerName { get; private set; } = $"Cosmos-DB-{Guid.NewGuid().ToString()}";

    public static async Task StartAsync(
        string? containerName = null)
    {
        if (IsRunning is false)
        {
            ContainerName = containerName ?? ContainerName;
            _containerInstance ??= new CosmosDbBuilder()
                .WithName(ContainerName)
                .WithAutoRemove(true)
                .WithPortBinding(CosmosDbBuilder.CosmosDbPort)
                .WithPortBinding(8900)
                .WithPortBinding(8901)
                .WithPortBinding(8902)
                .WithPortBinding(10250)
                .WithPortBinding(10251)
                .WithPortBinding(10252)
                .WithPortBinding(10253)
                .WithPortBinding(10254)
                .WithPortBinding(10255)
                .WithPortBinding(10256)
                .WithPortBinding(10350)
                .WithEnvironment(
                    "AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE",
                    GetLocalIpAddress())
                .WithWaitStrategy(
                    Wait
                        .ForUnixContainer()
                        .UntilPortIsAvailable(CosmosDbBuilder.CosmosDbPort))
                .Build();

            await _containerInstance.StartAsync();
            IsRunning = true;
        }
    }

    private static string GetLocalIpAddress()
    {
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }

        throw new Exception("No network adapters with an IPv4 address in the system!");
    }

    public static async Task StopAsync()
    {
        if (IsRunning && _containerInstance is not null)
        {
            await _containerInstance.StopAsync();
            await _containerInstance.DisposeAsync();
            _containerInstance = null;
        }

        IsRunning = false;
    }

    public static string ConnectionString =>
        _containerInstance?.GetConnectionString() ??
        throw new InvalidOperationException(
            "Cannot get the connection string as the cosmos emulator has not been initialised");
}