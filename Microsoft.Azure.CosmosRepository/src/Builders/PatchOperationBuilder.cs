// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Microsoft.Azure.CosmosRepository.Builders
{
    internal class PatchOperationBuilder<TItem> : IPatchOperationBuilder<TItem> where TItem : IItem
    {
        private readonly List<PatchOperation> _patchOperations = new();
        private readonly CamelCaseNamingStrategy _namingStrategy = new();

        public IReadOnlyList<PatchOperation> PatchOperations => _patchOperations;


        public IPatchOperationBuilder<TItem> Replace<TValue>(Expression<Func<TItem, TValue>> expression, TValue value)
        {
            PropertyInfo property = GetPropertyInfo(expression);
            string propertyToReplace = GetPropertyToReplace(property);

            _patchOperations.Add(PatchOperation.Replace($"/{propertyToReplace}", value));
            return this;
        }

        private string GetPropertyToReplace(PropertyInfo propertyInfo)
        {
            object[] attributes = propertyInfo.GetCustomAttributes(true);

            if (attributes.Any() is false)
            {
                return _namingStrategy.GetPropertyName(propertyInfo.Name, false);
            }

            foreach (object attribute in attributes)
            {
                JsonPropertyAttribute jsonAttribute = (JsonPropertyAttribute) attribute;
                if (jsonAttribute is not null)
                {
                    return jsonAttribute.PropertyName;
                }
            }

            return _namingStrategy.GetPropertyName(propertyInfo.Name, false);
        }

        private PropertyInfo GetPropertyInfo<TSource, TProperty>(Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);

            MemberExpression member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a method, not a property.");

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");

            if (propInfo.ReflectedType != null && type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a property that is not from type {type}.");

            return propInfo;
        }
    }
}