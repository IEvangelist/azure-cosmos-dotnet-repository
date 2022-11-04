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
        private readonly NamingStrategy _namingStrategy;

        internal readonly List<InternalPatchOperation> _rawPatchOperations = new();

        public IReadOnlyList<PatchOperation> PatchOperations => _patchOperations;

        public PatchOperationBuilder() =>
            _namingStrategy = new CamelCaseNamingStrategy();

        public PatchOperationBuilder(CosmosPropertyNamingPolicy? cosmosPropertyNamingPolicy) => 
            _namingStrategy = cosmosPropertyNamingPolicy == CosmosPropertyNamingPolicy.Default
                ? new DefaultNamingStrategy()
                : new CamelCaseNamingStrategy();

        public IPatchOperationBuilder<TItem> Replace<TValue>(Expression<Func<TItem, TValue>> expression, TValue? value)
        {
            PropertyInfo property = expression.GetPropertyInfo();
            string propertyToReplace = GetPropertyToReplace(property);
            _rawPatchOperations.Add(new InternalPatchOperation(property, value, PatchOperationType.Replace));
            _patchOperations.Add(PatchOperation.Replace($"/{propertyToReplace}", value));
            return this;
        }

        public IPatchOperationBuilder<TItem> Set<TValue>(Expression<Func<TItem, TValue>> expression, TValue? value)
        {
            PropertyInfo property = expression.GetPropertyInfo();
            string propertyToReplace = GetPropertyToReplace(property);
            _rawPatchOperations.Add(new InternalPatchOperation(property, value, PatchOperationType.Set));
            _patchOperations.Add(PatchOperation.Set($"/{propertyToReplace}", value));
            return this;
        }

        public IPatchOperationBuilder<TItem> Add<TValue>(Expression<Func<TItem, TValue>> expression, TValue? value)
        {
            PropertyInfo property = expression.GetPropertyInfo();
            string propertyToReplace = GetPropertyToReplace(property);
            _rawPatchOperations.Add(new InternalPatchOperation(property, value, PatchOperationType.Replace));
            _patchOperations.Add(PatchOperation.Add($"/{propertyToReplace}", value));
            return this;
        }

        public IPatchOperationBuilder<TItem> Set<TValue>(string path, TValue? value)
        {
            _patchOperations.Add(PatchOperation.Set($"/{path}", value));
            return this;
        }

        public IPatchOperationBuilder<TItem> Add<TValue>(string path, TValue? value)
        {
            _patchOperations.Add(PatchOperation.Add($"/{path}", value));
            return this;
        }

        public IPatchOperationBuilder<TItem> Remove<TValue>(Expression<Func<TItem, TValue>> expression)
        {
            PropertyInfo property = expression.GetPropertyInfo();
            string propertyToReplace = GetPropertyToReplace(property);
            _rawPatchOperations.Add(new InternalPatchOperation(property, null, PatchOperationType.Remove));
            _patchOperations.Add(PatchOperation.Remove($"/{propertyToReplace}"));
            return this;
        }

        public IPatchOperationBuilder<TItem> Remove<TValue>(string path)
        {
            _patchOperations.Add(PatchOperation.Remove($"/{path}"));
            return this;
        }

        public IPatchOperationBuilder<TItem> Increment(string path, long value)
        {
            _patchOperations.Add(PatchOperation.Increment($"/{path}", value));
            return this;
        }

        public IPatchOperationBuilder<TItem> Increment(string path, double value)
        {
            _patchOperations.Add(PatchOperation.Increment($"/{path}", value));
            return this;
        }

        /// <summary>
        /// Get the property name to replace. This only works for a single level of nesting.
        /// </summary>
        /// <remarks>If you're looking for nesting call the respective method with a given path instead.</remarks>
        /// <param name="propertyInfo">The property to get the path of.</param>
        /// <returns>Returns the path of the property name.</returns>
        private string GetPropertyToReplace(MemberInfo propertyInfo)
        {
            JsonPropertyAttribute[] attributes =
                propertyInfo.GetCustomAttributes<JsonPropertyAttribute>(true).ToArray();

            return attributes.Length is 0
                ? _namingStrategy.GetPropertyName(propertyInfo.Name, false)
                : attributes[0].PropertyName;
        }
    }
}