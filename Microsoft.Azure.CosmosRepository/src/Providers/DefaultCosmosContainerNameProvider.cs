// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using Microsoft.Azure.CosmosRepository.Attributes;

namespace Microsoft.Azure.CosmosRepository.Providers
{
    /// <inheritdoc />
    class DefaultCosmosContainerNameProvider : ICosmosContainerNameProvider
    {
        static readonly Type _containerAttributeType = typeof(ContainerAttribute);
        static readonly ConcurrentDictionary<Type, string> _containerNameMap = new();

        /// <inheritdoc />
        public string GetContainerName<TItem>() where TItem : IItem =>
            _containerNameMap.GetOrAdd(typeof(TItem), GetContainerNameFactory);

        static string GetContainerNameFactory(Type type)
        {
            Attribute attribute =
                Attribute.GetCustomAttribute(type, _containerAttributeType);

            return attribute is ContainerAttribute containerAttribute
                ? containerAttribute.Name
                : type.Name;
        }
    }
}
