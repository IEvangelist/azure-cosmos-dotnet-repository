// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.Azure.CosmosRepository.Options
{
    /// <summary>
    /// Options for a container
    /// </summary>
    public class ContainerOptions
    {
        /// <summary>
        /// The <see cref="IItem"/> type the container options are for
        /// </summary>
        public Type Type { get; }


        /// <summary>
        /// Creates an instance of <see cref="ContainerOptions"/>
        /// </summary>
        /// <param name="type">The type of <see cref="IItem"/> the options are for</param>
        public ContainerOptions(Type type)
        {
            Type = type;
        }

        /// <summary>
        /// Name of the container
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// The partition key for the container
        /// </summary>
        public string PartitionKey { get; internal set; }

        /// <summary>
        /// Sets the name of the container
        /// </summary>
        /// <param name="name">The name of the container</param>
        /// <returns>Instance of <see cref="ContainerOptions"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public ContainerOptions WithContainer(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            return this;
        }


        /// <summary>
        /// Sets the partition key for the container
        /// </summary>
        /// <param name="partitionKey">The partition key for the container</param>
        /// <returns>Instance of <see cref="ContainerOptions"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public ContainerOptions WithPartitionKey(string partitionKey)
        {
            PartitionKey = partitionKey ?? throw new ArgumentNullException(nameof(partitionKey));
            return this;
        }
    }
}