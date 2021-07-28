// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Azure.CosmosRepository.Attributes;

namespace Microsoft.Azure.CosmosRepository.Providers
{
    /// <inheritdoc cref="Microsoft.Azure.CosmosRepository.Providers.ICosmosPartitionKeyPathProvider" />
    class DefaultCosmosPartitionKeyPathProvider :
        BaseCosmosAttributeConstraintProvider<string, PartitionKeyPathAttribute>,
        ICosmosPartitionKeyPathProvider
    {
        /// <inheritdoc />
        public string GetPartitionKeyPath<TItem>() where TItem : IItem => GetConstraint<TItem>();

        protected override string GetConstraintFactory((Type ConstraintAttributeType, Type Type) key) =>
            Attribute.GetCustomAttribute(
                key.Type, key.ConstraintAttributeType) is PartitionKeyPathAttribute partitionKeyPathAttribute
                ? partitionKeyPathAttribute.Path
                : "/id";
    }
}
