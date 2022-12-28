// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Extensions;

internal static class TypeExtensions
{
    public static void IsItem(this Type type)
    {
        if (!typeof(IItem).IsAssignableFrom(type))
        {
            throw new InvalidOperationException(
                $"The type {type.FullName} does not implement {typeof(IItem).FullName}");
        }
    }

    public static void AreAllItems(this IReadOnlyList<Type> types)
    {
        foreach (Type type in types) type.IsItem();
    }
}