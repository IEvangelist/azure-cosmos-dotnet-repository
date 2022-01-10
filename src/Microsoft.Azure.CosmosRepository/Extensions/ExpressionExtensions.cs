﻿// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Azure.CosmosRepository.Extensions
{
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
            Dictionary<ParameterExpression, ParameterExpression> map =
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

            if (propInfo.ReflectedType != null && type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a property that is not from type {type}.");

            return propInfo;
        }
    }
}