// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Extensions;
using Microsoft.Azure.CosmosRepository.Internals;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Microsoft.Azure.CosmosRepository.Builders
{
    internal class PatchOperationBuilder<TItem> : IPatchOperationBuilder<TItem> where TItem : IItem
    {
        private readonly List<PatchOperation> _patchOperations = new();
        private readonly CamelCaseNamingStrategy _namingStrategy = new();

        internal readonly List<InternalPatchOperation> _rawPatchOperations = new();

        public IReadOnlyList<PatchOperation> PatchOperations => _patchOperations;

        public IPatchOperationBuilder<TItem> Replace<TValue>(Expression<Func<TItem, TValue>> expression, TValue value)
        {
            PropertyInfo property = expression.GetPropertyInfo();
            string propertyToReplace = GetPropertyToReplace(property);
            _rawPatchOperations.Add(new InternalPatchOperation(property, value, PatchOperationType.Replace));
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
    }
}