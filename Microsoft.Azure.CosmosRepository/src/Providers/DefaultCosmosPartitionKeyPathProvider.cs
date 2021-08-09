// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using Microsoft.Azure.CosmosRepository.Attributes;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.CosmosRepository.Providers
{
    /// <inheritdoc cref="Microsoft.Azure.CosmosRepository.Providers.ICosmosPartitionKeyPathProvider" />
    class DefaultCosmosPartitionKeyPathProvider :
        BaseCosmosAttributeConstraintProvider<string, PartitionKeyPathAttribute>,
        ICosmosPartitionKeyPathProvider
    {
        private readonly IOptions<RepositoryOptions> _options;

        public DefaultCosmosPartitionKeyPathProvider(IOptions<RepositoryOptions> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <inheritdoc />
        public string GetPartitionKeyPath<TItem>() where TItem : IItem => GetConstraint<TItem>();

        protected override string GetConstraintFactory((Type ConstraintAttributeType, Type Type) key)
        {
            ContainerOptions options = _options.Value.ContainerOptions.FirstOrDefault(opts => opts.Type == key.Type);

            if (options is { } && string.IsNullOrWhiteSpace(options.PartitionKey) is false)
            {
                return options.PartitionKey;
            }

            return Attribute.GetCustomAttribute(
                key.Type, key.ConstraintAttributeType) is PartitionKeyPathAttribute partitionKeyPathAttribute
                ? partitionKeyPathAttribute.Path
                : "/id";
        }
    }
}
