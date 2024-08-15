// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.IO;
using System.Linq.Expressions;
using System.Reflection;

[assembly: InternalsVisibleTo("Microsoft.Azure.CosmosRepositoryTests")]
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
            var propertyInfo = expression.GetPropertyInfo();
            var propertyToReplace = GetPropertyToReplace(propertyInfo);
            return InternalReplace($"/{propertyToReplace}", propertyInfo, value);
        }
        public IPatchOperationBuilder<TItem> Replace<TValue>(string path, TValue value)
        {
            var propertyInfo = GetPropertyInfoToReplace(path);
            return InternalReplace(path, propertyInfo, value);
        }

        public IPatchOperationBuilder<TItem> Set<TValue>(Expression<Func<TItem, TValue>> expression, TValue? value)
        {
            var propertyInfo = expression.GetPropertyInfo();
            var propertyToReplace = GetPropertyToReplace(propertyInfo);
            return InternalSet($"/{propertyToReplace}", propertyInfo, value);
        }

        public IPatchOperationBuilder<TItem> Set<TValue>(string path, TValue? value)
        {
            var propertyInfo = GetPropertyInfoToReplace(path);
            return InternalSet(path, propertyInfo, value);
        }

        public IPatchOperationBuilder<TItem> Add<TValue>(Expression<Func<TItem, TValue>> expression, TValue? value)
        {
            var propertyInfo = expression.GetPropertyInfo();
            var propertyToReplace = GetPropertyToReplace(propertyInfo);
            return InternalAdd($"/{propertyToReplace}", propertyInfo, value);
        }
        public IPatchOperationBuilder<TItem> Add<TValue>(string path, TValue? value)
        {
            var propertyInfo = GetPropertyInfoToReplace(path);
            var propertyToReplace = GetPropertyToReplace(propertyInfo);
            return InternalAdd(propertyToReplace, propertyInfo, value);
        }

        public IPatchOperationBuilder<TItem> Remove<TValue>(Expression<Func<TItem, TValue>> expression)
        {
            var propertyInfo = expression.GetPropertyInfo();
            var propertyToReplace = GetPropertyToReplace(propertyInfo);
            return InternalRemove($"/{propertyToReplace}", propertyInfo);
        }

        public IPatchOperationBuilder<TItem> Remove(string path)
        {
            var propertyInfo = GetPropertyInfoToReplace(path);
            var propertyToReplace = GetPropertyToReplace(propertyInfo);
            return InternalRemove(propertyToReplace, propertyInfo);
        }

        public IPatchOperationBuilder<TItem> Increment<TValue>(Expression<Func<TItem, TValue>> expression, double value)
        {
            var propertyInfo = expression.GetPropertyInfo();
            var propertyToReplace = GetPropertyToReplace(propertyInfo);
            return InternalIncrement($"/{propertyToReplace}", propertyInfo, value);
        }

        public IPatchOperationBuilder<TItem> Increment<TValue>(Expression<Func<TItem, TValue>> expression, long value)
        {
            var propertyInfo = expression.GetPropertyInfo();
            var propertyToReplace = GetPropertyToReplace(propertyInfo);
            return InternalIncrement($"/{propertyToReplace}", propertyInfo, value);
        }

        public IPatchOperationBuilder<TItem> Increment(string path, long value)
        {
            var propertyInfo = GetPropertyInfoToReplace(path);
            var propertyToReplace = GetPropertyToReplace(propertyInfo);
            return InternalIncrement(propertyToReplace, propertyInfo, value);
        }

        public IPatchOperationBuilder<TItem> Increment(string path, double value)
        {
            var propertyInfo = GetPropertyInfoToReplace(path);
            var propertyToReplace = GetPropertyToReplace(propertyInfo);
            return InternalIncrement(propertyToReplace, propertyInfo, value);
        }

        public IPatchOperationBuilder<TItem> InternalReplace<TValue>(string path, PropertyInfo property, TValue? value)
        {
            _rawPatchOperations.Add(new InternalPatchOperation(property, value, PatchOperationType.Replace));
            _patchOperations.Add(PatchOperation.Replace($"{path}", value));
            return this;
        }

        private IPatchOperationBuilder<TItem> InternalSet<TValue>(string path, PropertyInfo property, TValue? value)
        {
            _rawPatchOperations.Add(new InternalPatchOperation(property, value, PatchOperationType.Set));
            _patchOperations.Add(PatchOperation.Set(path, value));
            return this;
        }

        public IPatchOperationBuilder<TItem> InternalIncrement(string path, PropertyInfo property, long value)
        {
            _rawPatchOperations.Add(new InternalPatchOperation(property, value, PatchOperationType.Increment));
            _patchOperations.Add(PatchOperation.Increment(path, value));
            return this;
        }

        public IPatchOperationBuilder<TItem> InternalIncrement(string path, PropertyInfo property, double value)
        {
            _rawPatchOperations.Add(new InternalPatchOperation(property, value, PatchOperationType.Increment));
            _patchOperations.Add(PatchOperation.Increment(path, value));
            return this;
        }

        public IPatchOperationBuilder<TItem> InternalAdd<TValue>(string path, PropertyInfo property, TValue? value)
        {
            _rawPatchOperations.Add(new InternalPatchOperation(property, value, PatchOperationType.Add));
            _patchOperations.Add(PatchOperation.Add(path, value));
            return this;
        }

        public IPatchOperationBuilder<TItem> InternalRemove(string path, PropertyInfo property)
        {
            _rawPatchOperations.Add(new InternalPatchOperation(property, null, PatchOperationType.Remove));
            _patchOperations.Add(PatchOperation.Remove(path));
            return this;
        }

        /// <summary>
        /// Get the property name to replace. This only works for a single level of nesting.
        /// </summary>
        /// <remarks>If you're looking for nesting call the respective method with a given path instead.</remarks>
        /// <param name="propertyInfo">The property info of the property to be replaced.</param>
        /// <returns>Returns the path of the property name.</returns>
        internal string GetPropertyToReplace(MemberInfo propertyInfo)
        {
            JsonPropertyAttribute[] attributes =
                propertyInfo.GetCustomAttributes<JsonPropertyAttribute>(true).ToArray();

            return attributes.Length is 0
                ? _namingStrategy.GetPropertyName(propertyInfo.Name, false)
                : attributes[0].PropertyName;
        }

        /// <summary>
        /// Get the property name to replace. This only works for a single level of nesting.
        /// </summary>
        /// <remarks>If you're looking for nesting call the respective method with a given path instead.</remarks>
        /// <param name="path">The path to get the property.</param>
        /// <returns>Returns the path of the property name.</returns>
        internal PropertyInfo GetPropertyInfoToReplace(string path)
        {
            Type itemType = typeof(TItem);

            PropertyInfo property = GetNestedPropertyInfo(itemType, path) ?? throw new InvalidOperationException($"The property {path} does not exist on {itemType.Name}");

            return property;
        }

        public static PropertyInfo? GetPropertyInfoByJsonPropertyName(Type type, string jsonPropertyName)
        {
            // Iterate through all properties of the type
            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                // Check if the property has a JsonPropertyAttribute
                var jsonPropertyAttribute = property.GetCustomAttribute<JsonPropertyAttribute>();

                // If the attribute is found and the PropertyName matches the provided jsonPropertyName
                if (jsonPropertyAttribute != null && jsonPropertyAttribute.PropertyName == jsonPropertyName)
                {
                    // Return the PropertyInfo of the matching property
                    return property;
                }
            }

            // Return null if no matching property is found
            return null;
        }

        public static PropertyInfo? GetNestedPropertyInfo(Type type, string path)
        {
            // Split the path by '/' to get the individual property names
            string[] properties = path.Split('/');

            Type currentType = type;
            PropertyInfo? propertyInfo = null;

            // Iterate through each property in the path
            foreach (string propertyNameOrJsonProperty in properties)
            {
                // First, attempt to get the property by its direct name
                propertyInfo = currentType.GetProperty(propertyNameOrJsonProperty, BindingFlags.Public | BindingFlags.Instance);

                // If the property is found by name, continue to the next level
                if (propertyInfo != null)
                {
                    currentType = propertyInfo.PropertyType;
                    continue;
                }

                // If the property is not found by name, search by JsonPropertyAttribute
                PropertyInfo[] currentProperties = currentType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (var property in currentProperties)
                {
                    var jsonPropertyAttribute = property.GetCustomAttribute<JsonPropertyAttribute>();
                    if (jsonPropertyAttribute != null && jsonPropertyAttribute.PropertyName == propertyNameOrJsonProperty)
                    {
                        propertyInfo = property;
                        currentType = propertyInfo.PropertyType;
                        break;
                    }
                }

                // If no matching property is found at all, return null
                if (propertyInfo == null)
                {
                    return null;
                }
            }

            // Return the final PropertyInfo
            return propertyInfo;
        }

    }
}