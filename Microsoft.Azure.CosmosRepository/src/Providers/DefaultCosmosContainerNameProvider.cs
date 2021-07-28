// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using Microsoft.Azure.CosmosRepository.Attributes;
using Microsoft.Azure.CosmosRepository.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Azure.CosmosRepository.Providers
{
    /// <inheritdoc />
    class  DefaultCosmosContainerNameProvider : ICosmosContainerNameProvider
    {
        private readonly IServiceProvider _serviceProvider;
        static readonly Type ContainerAttributeType = typeof(ContainerAttribute);
        static readonly ConcurrentDictionary<Type, string> ContainerNameMap = new();
        private static IItemContainerBuilder _container;

        public DefaultCosmosContainerNameProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public string GetContainerName<TItem>() where TItem : IItem =>
            ContainerNameMap.GetOrAdd(typeof(TItem), GetContainerNameFactory);

        private string GetContainerNameFactory(Type type)
        {
            Attribute attribute =
                Attribute.GetCustomAttribute(type, ContainerAttributeType);

            _container ??= _serviceProvider.GetService<IItemContainerBuilder>();

            if (_container is { } && _container.Options.ContainsKey(type))
            {
                return _container.Options[type].Name;
            }

            return attribute is ContainerAttribute containerAttribute
                ? containerAttribute.Name
                : type.Name;
        }
    }
}
