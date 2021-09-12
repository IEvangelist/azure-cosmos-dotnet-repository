// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.Azure.CosmosRepository.Builders
{
    /// <summary>
    /// Options for a container
    /// </summary>
    public class ContainerOptionsBuilder
    {
        /// <summary>
        /// The <see cref="IItem"/> type the container options are for
        /// </summary>
        internal Type Type { get; }

        /// <summary>
        /// Creates an instance of <see cref="ContainerOptionsBuilder"/>.
        /// </summary>
        /// <param name="type">The type of <see cref="IItem"/> the options are for.</param>
        public ContainerOptionsBuilder(Type type) => Type = type;

        /// <summary>
        /// Name of the container.
        /// </summary>
        internal string Name { get; private set; }

        /// <summary>
        /// The partition key for the container.
        /// </summary>
        internal string PartitionKey { get; private set; }

        /// <summary>
        /// The default time to live for a container.
        /// </summary>
        /// <remarks>If <see cref="Item"/> share a container they will share this property.</remarks>
        internal TimeSpan? ContainerDefaultTimeToLive { get; private set; }

        /// <summary>
        /// Sets the <see cref="ContainerDefaultTimeToLive"/> for a container.
        /// </summary>
        /// <param name="containerDefaultTimeToLive">The default time to live for the container.</param>
        /// <returns></returns>
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
            PartitionKey = partitionKey ?? throw new ArgumentNullException(nameof(partitionKey));
            return this;
        }
    }
}
