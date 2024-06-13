// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Builders;

/// <summary>
/// Options for a container
/// </summary>
/// <remarks>
/// Creates an instance of <see cref="ContainerOptionsBuilder"/>.
/// </remarks>
/// <param name="type">The type of <see cref="IItem"/> the options are for.</param>
public class ContainerOptionsBuilder(Type type)
{
    /// <summary>
    /// The <see cref="IItem"/> type the container options are for
    /// </summary>
    internal Type Type { get; } = type;

    /// <summary>
    /// Name of the container.
    /// </summary>
    internal string? Name { get; private set; }

    /// <summary>
    /// The partition keys for the container.
    /// </summary>
    internal IList<string>? PartitionKeys { get; private set; }

    /// <summary>
    /// The default time to live for a container.
    /// </summary>
    /// <remarks>If <see cref="Item"/> share a container they will share this property.</remarks>
    internal TimeSpan? ContainerDefaultTimeToLive { get; private set; }

    /// <summary>
    /// Syncs the container properties when the container is first created.
    /// </summary>
    /// <remarks>This can sync settings such as <see cref="ContainerDefaultTimeToLive"/></remarks>
    internal bool SyncContainerProperties { get; private set; }

    /// <summary>
    /// The <see cref="ThroughputProperties"/> for the given container.
    /// </summary>
    /// <remarks>By default this uses a manual throughput reserved at 400 RU/s in line with the Cosmos SDK.</remarks>
    internal ThroughputProperties? ThroughputProperties { get; private set; } = ThroughputProperties.CreateManualThroughput(400);

    internal ChangeFeedOptions? ChangeFeedOptions { get; private set; } = null;

    internal bool UseStrictTypeChecking { get; set; } = true;

    /// <summary>
    /// Sets the <see cref="ContainerDefaultTimeToLive"/> for a container.
    /// </summary>
    /// <param name="containerDefaultTimeToLive">The default time to live for the container.</param>
    public ContainerOptionsBuilder WithContainerDefaultTimeToLive(TimeSpan containerDefaultTimeToLive)
    {
        ContainerDefaultTimeToLive = containerDefaultTimeToLive;
        return this;
    }

    /// <summary>
    /// Sets the <see cref="Name"/> of the container
    /// </summary>
    /// <param name="name">The name of the container</param>
    /// <returns>Instance of <see cref="ContainerOptionsBuilder"/></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public ContainerOptionsBuilder WithContainer(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        return this;
    }

    /// <summary>
    /// Sets the partition key for the container
    /// </summary>
    /// <param name="partitionKey">The partition key for the container</param>
    /// <returns>Instance of <see cref="ContainerOptionsBuilder"/></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public ContainerOptionsBuilder WithPartitionKey(string partitionKey)
    {
        if (partitionKey is null) throw new ArgumentNullException(nameof(partitionKey));
        
        PartitionKeys ??= [];
        PartitionKeys.Add(partitionKey);
        
        return this;
    }

    /// <summary>
    /// Sets <see cref="SyncContainerProperties"/> to true
    /// </summary>
    /// <returns>Instance of <see cref="ContainerOptionsBuilder"/></returns>
    public ContainerOptionsBuilder WithSyncableContainerProperties()
    {
        SyncContainerProperties = true;
        return this;
    }

    /// <summary>
    /// Sets the container for this <see cref="IItem"/> to use a manual throughput value.
    /// </summary>
    /// <param name="throughput">The RU/s that this container can utilise.</param>
    /// <remarks>This value must be at least 400 RU/s.</remarks>
    /// <remarks>If a container has already been created without specifying a throughput then it cannot be updated.</remarks>
    /// <exception cref="InvalidOperationException">When the RU/s is less than 400.</exception>
    /// <returns>Instance of <see cref="ContainerOptionsBuilder"/></returns>
    public ContainerOptionsBuilder WithManualThroughput(int throughput = 400)
    {
        if (throughput < 400)
        {
            throw new ArgumentOutOfRangeException(nameof(throughput), "A container must at least set a throughput level of 400 RU/s");
        }

        ThroughputProperties = ThroughputProperties.CreateManualThroughput(throughput);
        return this;
    }

    /// <summary>
    /// Sets the container for this <see cref="IItem"/> to use a autoscale throughput value.
    /// </summary>
    /// <param name="maxAutoScaleThroughput">The maximum RU/s that this containers throughput can autoscale to.</param>
    /// <remarks>If a container has already been created without specifying a throughput then it cannot be updated.</remarks>
    /// <returns>Instance of <see cref="ContainerOptionsBuilder"/></returns>
    public ContainerOptionsBuilder WithAutoscaleThroughput(int maxAutoScaleThroughput = 4_000)
    {
        if (maxAutoScaleThroughput is < 4_000 or > 1_000_000)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxAutoScaleThroughput),
                "Autoscale throughput must be between 4,000 and 1,000,000 RUs.");
        }

        if (maxAutoScaleThroughput % 1000 != 0)
        {
            throw new InvalidOperationException("Autoscale throughput must be defined in increments of 1,000");
        }

        ThroughputProperties = ThroughputProperties.CreateAutoscaleThroughput(maxAutoScaleThroughput);
        return this;
    }

    /// <summary>
    /// When your Cosmos DB resource is
    /// <a href="https://docs.microsoft.com/azure/cosmos-db/serverless?WC.m_id=dapine">
    /// configured for serverless</a>, your containers must explicitly set
    /// serverless <see cref="ThroughputProperties"/>.
    /// </summary>
    /// <returns>Instance of <see cref="ContainerOptionsBuilder"/></returns>
    public ContainerOptionsBuilder WithServerlessThroughput()
    {
        ThroughputProperties = null;
        return this;
    }

    /// <summary>
    /// Adds monitoring of the change feed for the given <see cref="IItem"/>
    /// </summary>
    /// <param name="optionsActions">An action to configure the change feed for the given container.</param>
    /// <returns>Instance of <see cref="ContainerOptionsBuilder"/></returns>
    /// <remarks>The options configured here are for the container, not just the <see cref="IItem"/> be aware if item's share a container they will share the same change feed options.</remarks>
    public ContainerOptionsBuilder WithChangeFeedMonitoring(Action<ChangeFeedOptions>? optionsActions = null)
    {
        ChangeFeedOptions options = new(Type);

        optionsActions?.Invoke(options);

        ChangeFeedOptions = options;

        return this;
    }

    /// <summary>
    /// Configures the given <see cref="IItem"/>'s queries to not check for the Type field.
    /// </summary>
    /// <remarks>This is useful in scenarios where you have a sub types that will deserialize into a base type.</remarks>
    /// <returns>Instance of <see cref="ContainerOptionsBuilder"/></returns>
    public ContainerOptionsBuilder WithoutStrictTypeChecking()
    {
        UseStrictTypeChecking = false;
        return this;
    }
}
