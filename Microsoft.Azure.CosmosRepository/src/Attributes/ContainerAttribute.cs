// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Azure.CosmosRepository.Options;

namespace Microsoft.Azure.CosmosRepository.Attributes
{
    /// <summary>
    /// The container attribute exposes the ability to declaratively
    /// specify an <see cref="IItem"/>'s container name. This attribute can only be used when
    /// <see cref="RepositoryOptions.ContainerPerItemType"/> is set to <c>true</c>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ContainerAttribute : Attribute
    {
        /// <summary>
        /// Gets the path of the parition key.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Constructor accepting the <paramref name="name"/> of the container for a given <see cref="IItem"/>.
        /// </summary>
        /// <param name="name"></param>
        public ContainerAttribute(string name) =>
            Name = name ?? throw new ArgumentNullException(nameof(name), "A name is required.");
    }
}
