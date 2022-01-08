// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;

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
    }
}