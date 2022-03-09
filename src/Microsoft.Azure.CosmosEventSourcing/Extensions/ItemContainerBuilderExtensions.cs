// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Builders;

namespace Microsoft.Azure.CosmosEventSourcing.Extensions;

/// <summary>
/// Extensions to enhance the <see cref="IItemContainerBuilder"/>
/// </summary>
public static class ItemContainerBuilderExtensions
{
    /// <summary>
    /// Configures an event source container.
    /// </summary>
    /// <param name="containerBuilder">The builder used to customise a containers.</param>
    /// <param name="containerName">The name of the container to store the events.</param>
    /// <param name="containerOptionsBuilder">The options to build the container.</param>
    /// <typeparam name="TEventItem">The type of <see cref="EventItem"/> to store in this container.</typeparam>
    /// <returns></returns>
    public static IItemContainerBuilder ConfigureEventItemStore<TEventItem>(
        this IItemContainerBuilder containerBuilder,
        string containerName,
        Action<ContainerOptionsBuilder>? containerOptionsBuilder = null)
        where TEventItem : EventItem
    {
        containerBuilder.Configure<TEventItem>(options =>
        {
            options.WithContainer(containerName);
            options.WithPartitionKey(CosmosEventSourcingPartitionKeys.Default);
            containerOptionsBuilder?.Invoke(options);
        });

        return containerBuilder;
    }

    /// <summary>
    /// Configures a container to store projections.
    /// </summary>
    /// <param name="containerBuilder">The builder used to customise a containers.</param>
    /// <param name="containerName">The name of the container to store the events.</param>
    /// <param name="partitionKey">The partition key used to partition this projection.</param>
    /// <param name="containerOptionsBuilder">The options to build the container.</param>
    /// <typeparam name="TProjection">The <see cref="IItem"/> that represents the projection.</typeparam>
    /// <returns></returns>
    public static IItemContainerBuilder ConfigureProjectionStore<TProjection>(
        this IItemContainerBuilder containerBuilder,
        string containerName,
        string partitionKey = CosmosEventSourcingPartitionKeys.Default,
        Action<ContainerOptionsBuilder>? containerOptionsBuilder = default)
        where TProjection : IItem =>
        containerBuilder.Configure<TProjection>(options =>
        {
            options.WithContainer(containerName);
            options.WithPartitionKey(partitionKey);
            containerOptionsBuilder?.Invoke(options);
        });
}