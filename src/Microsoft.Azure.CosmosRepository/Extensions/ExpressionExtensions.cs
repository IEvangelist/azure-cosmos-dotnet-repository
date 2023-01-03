// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Extensions;

/// <summary>
/// Borrowed from:
/// https://docs.microsoft.com/en-us/archive/blogs/meek/linq-to-entities-combining-predicates
/// </summary>
internal static class ExpressionExtensions
{
    internal static Expression<T> Compose<T>(
        this Expression<T> first,
        Expression<T> second,
        Func<Expression, Expression, Expression> merge)
    {
        var map =
            first.Parameters
                .Select((parameter, index) => (parameter, second: second.Parameters[index]))
                .ToDictionary(p => p.second, p => p.parameter);

        Expression secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

        return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);

    }

    internal static Expression<Func<T, bool>> AndAlso<T>(
        this Expression<Func<T, bool>> first,
        Expression<Func<T, bool>> second) => first.Compose(second, Expression.AndAlso);

    internal static Expression<Func<T, bool>> And<T>(
        this Expression<Func<T, bool>> first,
        Expression<Func<T, bool>> second) => first.Compose(second, Expression.And);

    internal static Expression<Func<T, bool>> Or<T>(
        this Expression<Func<T, bool>> first,
        Expression<Func<T, bool>> second) => first.Compose(second, Expression.Or);

    internal static PropertyInfo GetPropertyInfo<TSource, TProperty>(this Expression<Func<TSource, TProperty>> propertyLambda)
    {
        Type type = typeof(TSource);

        if (propertyLambda.Body is not MemberExpression member)
            throw new ArgumentException($"Expression '{propertyLambda}' refers to a method, not a property.");

        if (member.Member is not PropertyInfo propInfo)
            throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");

#pragma warning disable IDE0046 // Convert to conditional expression
        if (propInfo.ReflectedType != null &&
            type != propInfo.ReflectedType &&
            !type.IsSubclassOf(propInfo.ReflectedType))
            throw new ArgumentException($"Expression '{propertyLambda}' refers to a property that is not from type {type}.");
#pragma warning restore IDE0046 // Convert to conditional expression

        return propInfo;
    }
}