// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Reflection;

namespace Microsoft.Azure.CosmosEventSourcingTests.Extensions;

public static class ObjectExtensions
{
    public static void SetPrivatePropertyValue<T>(this object obj, string propName, T val)
    {
        Type t = obj.GetType();

        PropertyInfo? prop = t.GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (prop == null)
        {
            throw new ArgumentOutOfRangeException(
                nameof(propName),
                $"Property {propName} was not found in Type {obj.GetType().FullName}");
        }

        MethodInfo m = prop.DeclaringType!.GetMethod(
            $"set_{propName}",
            BindingFlags.NonPublic | BindingFlags.Instance)!;

        m.Invoke(
            obj,
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            [val!],
            null);
    }
}