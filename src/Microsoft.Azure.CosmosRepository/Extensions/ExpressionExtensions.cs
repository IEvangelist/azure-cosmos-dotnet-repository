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


    //internal static PropertyInfo GetPropertyInfo<TSource, TProperty>(this Expression<Func<TSource, TProperty>> propertyLambda)
    //{
    //    //var propertyInfos = GetPropertyInfosInternal(propertyLambda);

    //    //PropertyInfo propInfo = propertyInfos[propertyInfos.Count - 1];

    //    //ThrowArgumentExceptionIfPropertyIsNotFromSourceType(propertyLambda, propInfo);
      
    //    //Intermediate Step Our Code
    //      // Check if the body is a MemberExpression
    //      if (propertyLambda.Body is MemberExpression memberExpression)
    //      {
    //          // Check if the MemberExpression refers to a Property
    //          if (memberExpression.Member is PropertyInfo propInfo)
    //          {
    //              return propInfo;
    //          }
    //          else
    //          {
    //              throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");
    //          }
    //      }

    //      if (propertyLambda.Body.NodeType == ExpressionType.ArrayIndex && propertyLambda.Body is BinaryExpression binaryExpression && binaryExpression.Left is MemberExpression binaryMemberExpression)
    //      {
    //          // Check if the MemberExpression refers to a Property
    //          if (binaryMemberExpression.Member is PropertyInfo propInfo)
    //          {
    //              return propInfo;
    //          }
    //          else
    //          {
    //              throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");
    //          }
    //      }
    //      // Handle unexpected cases
    //      throw new ArgumentException($"Expression '{propertyLambda}' is not a valid property expression.");

    //    // return propInfo;
    //}

    internal static IReadOnlyList<PropertyInfo> GetPropertyInfos<TSource, TProperty>(this Expression<Func<TSource, TProperty>> propertyLambda)
    {
        List<PropertyInfo> propertyInfos = GetPropertyInfosInternal(propertyLambda);

        PropertyInfo propInfo = propertyInfos[0];

        ThrowArgumentExceptionIfPropertyIsNotFromSourceType(propertyLambda, propInfo);

        return propertyInfos;
    }

    private static List<PropertyInfo> GetPropertyInfosInternal<TSource, TProperty>(Expression<Func<TSource, TProperty>> propertyLambda)
    {
        List<PropertyInfo> propertyInfos = [];

        MemberExpression? member = propertyLambda.Body as MemberExpression;

        if (member == null)
        {
            throw new ArgumentException($"Expression '{propertyLambda}' refers to a method, not a property.");
        }

        while (member != null)
        {
            if (member.Member is not PropertyInfo propertyInfo)
            {
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");
            }

            propertyInfos.Add(propertyInfo);

            member = member.Expression as MemberExpression;
        }

        propertyInfos.Reverse(); // The properties are added from the leaf to the root, so we reverse to get them in the correct order.

        return propertyInfos;
    }

    private static void ThrowArgumentExceptionIfPropertyIsNotFromSourceType<TSource, TProperty>(
        Expression<Func<TSource, TProperty>> propertyLambda,
        PropertyInfo propertyInfo)
    {
        var type = typeof(TSource);

#pragma warning disable IDE0046 // Convert to conditional expression
        if (propertyInfo.ReflectedType != null &&
            type != propertyInfo.ReflectedType &&
            !type.IsSubclassOf(propertyInfo.ReflectedType))
        {
            throw new ArgumentException($"Expression '{propertyLambda}' refers to a property that is not from type {type}.");
        }
#pragma warning restore IDE0046 // Convert to conditional expression

    }
}