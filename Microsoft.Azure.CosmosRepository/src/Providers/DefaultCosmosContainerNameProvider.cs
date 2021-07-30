// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using Microsoft.Azure.CosmosRepository.Attributes;
using Microsoft.Azure.CosmosRepository.Builders;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.CosmosRepository.Providers
{
    /// <inheritdoc cref="Microsoft.Azure.CosmosRepository.Providers.ICosmosContainerNameProvider" />
    class  DefaultCosmosContainerNameProvider : BaseCosmosAttributeConstraintProvider<string, ContainerAttribute>, ICosmosContainerNameProvider
    {
        private readonly IOptions<RepositoryOptions> _options;
        static readonly ConcurrentDictionary<Type, string> ContainerNameMap = new();

        public DefaultCosmosContainerNameProvider(IOptions<RepositoryOptions> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <inheritdoc />
        public string GetContainerName<TItem>() where TItem : IItem => GetConstraint<TItem>();

        protected override string GetConstraintFactory((Type ConstraintAttributeType, Type Type) key)
        {
            Attribute attribute =
                Attribute.GetCustomAttribute(key.Type, key.ConstraintAttributeType);

            ContainerOptions options = _options.Value.ContainerOptions.FirstOrDefault(opts => opts.Type == key.Type);

            if (options is { } && string.IsNullOrWhiteSpace(options.Name) is false)
            {
                return options.Name;
            }

            return attribute is ContainerAttribute containerAttribute
                ? containerAttribute.Name
                : key.Type.Name;
        }
    }
}
