// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Linq.Expressions;

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
        // Check if the body is a MemberExpression
        if (propertyLambda.Body is MemberExpression memberExpression)
        {
            // Check if the MemberExpression refers to a Property
            if (memberExpression.Member is PropertyInfo propInfo)
            {
                return propInfo;
            }
            else
            {
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");
            }
        }

        if (propertyLambda.Body.NodeType == ExpressionType.ArrayIndex && propertyLambda.Body is BinaryExpression binaryExpression && binaryExpression.Left is MemberExpression binaryMemberExpression)
        {
            // Check if the MemberExpression refers to a Property
            if (binaryMemberExpression.Member is PropertyInfo propInfo)
            {
                return propInfo;
            }
            else
            {
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");
            }
        }
        // Handle unexpected cases
        throw new ArgumentException($"Expression '{propertyLambda}' is not a valid property expression.");
    }
}