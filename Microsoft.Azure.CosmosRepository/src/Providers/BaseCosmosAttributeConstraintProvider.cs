// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;

namespace Microsoft.Azure.CosmosRepository.Providers
{
    abstract class BaseCosmosAttributeConstraintProvider<TValue, TAttribute> where TAttribute : Attribute
    {
        static readonly ConcurrentDictionary<(Type, Type), TValue> _constraintMap = new();

        protected Type ConstraintAttributeType { get; } = typeof(TAttribute);

        protected TValue GetConstraint<TItem>() where TItem : IItem =>
            _constraintMap.GetOrAdd((ConstraintAttributeType, typeof(TItem)), GetConstraintFactory);

        protected abstract TValue GetConstraintFactory((Type ConstraintAttributeType, Type Type) key);
    }
}
