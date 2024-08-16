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
            var path = GetPath(expression);
            return InternalReplace(path, propertyInfo, value);
        }
        public IPatchOperationBuilder<TItem> Replace<TValue>(string path, TValue value)
        {
            var propertyInfo = GetPropertyInfoToReplace(path);

            return InternalReplace(path, propertyInfo, value);
        }

        public IPatchOperationBuilder<TItem> Set<TValue>(Expression<Func<TItem, TValue>> expression, TValue? value)
        {
            var propertyInfo = expression.GetPropertyInfo();
            var path = GetPath(expression);
            return InternalSet(path, propertyInfo, value);
        }

        public IPatchOperationBuilder<TItem> Set<TValue>(string path, TValue? value)
        {
            var propertyInfo = GetPropertyInfoToReplace(path);
            return InternalSet(path, propertyInfo, value);
        }

        public IPatchOperationBuilder<TItem> Add<TValue>(Expression<Func<TItem, TValue>> expression, TValue? value)
        {
            var propertyInfo = expression.GetPropertyInfo();
            var path = GetPath(expression);
            return InternalAdd(path, propertyInfo, value);
        }
        public IPatchOperationBuilder<TItem> Add<TValue>(string path, TValue? value)
        {
            var propertyInfo = GetPropertyInfoToReplace(path);
            return InternalAdd(path, propertyInfo, value);
        }

        public IPatchOperationBuilder<TItem> Remove<TValue>(Expression<Func<TItem, TValue>> expression)
        {
            var propertyInfo = expression.GetPropertyInfo();
            var path = GetPath(expression);
            return InternalRemove(path, propertyInfo);
        }

        public IPatchOperationBuilder<TItem> Remove(string path)
        {
            var propertyInfo = GetPropertyInfoToReplace(path);
            return InternalRemove(path, propertyInfo);
        }

        public IPatchOperationBuilder<TItem> Increment<TValue>(Expression<Func<TItem, TValue>> expression, double value)
        {
            var propertyInfo = expression.GetPropertyInfo();
            var path = GetPath(expression);
            return InternalIncrement(path, propertyInfo, value);
        }

        public IPatchOperationBuilder<TItem> Increment<TValue>(Expression<Func<TItem, TValue>> expression, long value)
        {
            var propertyInfo = expression.GetPropertyInfo();
            var path = GetPath(expression);
            return InternalIncrement(path, propertyInfo, value);
        }

        public IPatchOperationBuilder<TItem> Increment(string path, long value)
        {
            var propertyInfo = GetPropertyInfoToReplace(path);
            return InternalIncrement(path, propertyInfo, value);
        }

        public IPatchOperationBuilder<TItem> Increment(string path, double value)
        {
            var propertyInfo = GetPropertyInfoToReplace(path);
            var propertyToReplace = GetPropertyName(propertyInfo);
            return InternalIncrement(propertyToReplace, propertyInfo, value);
        }

        public IPatchOperationBuilder<TItem> InternalReplace<TValue>(string path, PropertyInfo property, TValue? value)
        {
            path = NormalizePath(path);
            _rawPatchOperations.Add(new InternalPatchOperation(property, value, PatchOperationType.Replace));
            _patchOperations.Add(PatchOperation.Replace(path, value));
            return this;
        }

        private IPatchOperationBuilder<TItem> InternalSet<TValue>(string path, PropertyInfo property, TValue? value)
        {
            path = NormalizePath(path);
            _rawPatchOperations.Add(new InternalPatchOperation(property, value, PatchOperationType.Set));
            _patchOperations.Add(PatchOperation.Set(path, value));
            return this;
        }

        public IPatchOperationBuilder<TItem> InternalIncrement(string path, PropertyInfo property, long value)
        {
            path = NormalizePath(path);
            _rawPatchOperations.Add(new InternalPatchOperation(property, value, PatchOperationType.Increment));
            _patchOperations.Add(PatchOperation.Increment(path, value));
            return this;
        }

        public IPatchOperationBuilder<TItem> InternalIncrement(string path, PropertyInfo property, double value)
        {
            path = NormalizePath(path);
            _rawPatchOperations.Add(new InternalPatchOperation(property, value, PatchOperationType.Increment));
            _patchOperations.Add(PatchOperation.Increment(path, value));
            return this;
        }

        public IPatchOperationBuilder<TItem> InternalAdd<TValue>(string path, PropertyInfo property, TValue? value)
        {
            path = NormalizePath(path);
            _rawPatchOperations.Add(new InternalPatchOperation(property, value, PatchOperationType.Add));
            _patchOperations.Add(PatchOperation.Add(path, value));
            return this;
        }

        public IPatchOperationBuilder<TItem> InternalRemove(string path, PropertyInfo property)
        {
            path = NormalizePath(path);
            _rawPatchOperations.Add(new InternalPatchOperation(property, null, PatchOperationType.Remove));
            _patchOperations.Add(PatchOperation.Remove(path));
            return this;
        }

        private string NormalizePath(string path) => "/" + path.TrimStart('/');

        /// <summary>
        /// Get the property name. This only works for a single level of nesting.
        /// </summary>
        /// <remarks>If you're looking for nesting call the respective method with a given path instead.</remarks>
        /// <param name="propertyInfo">The property info of the property to be replaced.</param>
        /// <returns>Returns the path of the property name.</returns>
        internal string GetPropertyName(MemberInfo propertyInfo)
        {
            JsonPropertyAttribute[] attributes =
                propertyInfo.GetCustomAttributes<JsonPropertyAttribute>(true).ToArray();

            return attributes.Length is 0
                ? _namingStrategy.GetPropertyName(propertyInfo.Name, false)
                : attributes[0].PropertyName;
        }

        public string GetPath<TValue>(Expression<Func<TItem, TValue>> expr)
        {
            var stack = new Stack<string>();

            MemberExpression? me;
            switch (expr.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    me = ((expr.Body is UnaryExpression ue) ? ue.Operand : null) as MemberExpression;
                    break;
                default:
                    me = expr.Body as MemberExpression;
                    break;
            }

            while (me != null)
            {
                var memberInfo = me.Member as PropertyInfo;

                if (memberInfo != null)
                {
                    var jsonPropertyAttribute = memberInfo.GetCustomAttribute<JsonPropertyAttribute>(true);

                    var propertyName = jsonPropertyAttribute != null
                        ? jsonPropertyAttribute.PropertyName
                        : _namingStrategy.GetPropertyName(memberInfo.Name, false);

                    stack.Push(propertyName);
                }

                me = me.Expression as MemberExpression;
            }

            var path = string.Join("/", stack);
            return NormalizePath(path); // Using the arrow function here
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

        private PropertyInfo? GetNestedPropertyInfo(Type type, string path)
        {
            // Split the path by '/' to get the individual property names
            string[] properties = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            Type currentType = type;
            PropertyInfo? propertyInfo = null;

            // Iterate through each property in the path
            foreach (string propertyNameOrJsonProperty in properties)
            {
                // Search for the property prioritizing JsonPropertyAttribute
                propertyInfo = FindProperty(currentType, propertyNameOrJsonProperty);

                // If no matching property is found, return null
                if (propertyInfo == null)
                {
                    return null;
                }

                // Move to the next level of nesting
                currentType = propertyInfo.PropertyType;
            }

            // Return the final PropertyInfo
            return propertyInfo;
        }

        private PropertyInfo? FindProperty(Type type, string propertyNameOrJsonProperty)
        {
            // Get all public instance properties of the current type once
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Standardize the provided property name
            var standardizedPropertyName = _namingStrategy.GetPropertyName(propertyNameOrJsonProperty, false);

            // Search for the property prioritizing JsonPropertyAttribute, fallback to direct name matching
            return properties.FirstOrDefault(p =>
            {
                var jsonPropertyAttribute = p.GetCustomAttribute<JsonPropertyAttribute>();
                if (jsonPropertyAttribute != null)
                {
                    return string.Equals(jsonPropertyAttribute.PropertyName, standardizedPropertyName, StringComparison.OrdinalIgnoreCase);
                }

                // Fallback to direct property name comparison (standardized for consistency)
                return string.Equals(_namingStrategy.GetPropertyName(p.Name, false), standardizedPropertyName, StringComparison.OrdinalIgnoreCase);
            });
        }

    }
}