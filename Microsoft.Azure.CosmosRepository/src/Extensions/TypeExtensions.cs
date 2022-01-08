// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.CosmosRepository.Extensions
{
    internal static class TypeExtensions
    {
        public static void EnsureIsTypeOfIItem(this Type type)
        {
            if(!typeof(IItem).IsAssignableFrom(type))
            {
                throw new InvalidOperationException(
                    $"The type {type.FullName} does not implement {typeof(IItem).FullName}");
            }
        }

        public static void EnsureAllAreTypeOfIItem(this IReadOnlyList<Type> types)
        {
            foreach (Type type in types) type.EnsureIsTypeOfIItem();
        }
    }
}