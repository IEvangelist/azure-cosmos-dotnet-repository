// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json.Linq;

[assembly: InternalsVisibleTo("Microsoft.Azure.CosmosRepositoryTests")]
namespace Microsoft.Azure.CosmosRepository.Builders
{


    internal class PatchOperationBuilder<TItem> : IPatchOperationBuilder<TItem> where TItem : IItem
    {
        private readonly List<PatchOperation> _patchOperations = [];
        private readonly NamingStrategy _namingStrategy;

        internal readonly List<InternalPatchOperation> _rawPatchOperations = [];

        public IReadOnlyList<PatchOperation> PatchOperations => _patchOperations;


        public PatchOperationBuilder() =>
            _namingStrategy = new CamelCaseNamingStrategy();
        public PatchOperationBuilder(CosmosPropertyNamingPolicy? cosmosPropertyNamingPolicy) =>
            _namingStrategy = cosmosPropertyNamingPolicy == CosmosPropertyNamingPolicy.Default
                ? new DefaultNamingStrategy()
                : new CamelCaseNamingStrategy();

        public IPatchOperationBuilder<TItem> Replace<TValue>(Expression<Func<TItem, TValue>> expression, TValue? value)
        {
            return InternalReplace(expression, value);
        }

        public IPatchOperationBuilder<TItem> Replace<TValue>(string path, TValue value)
        {
            var propertyInfos = GetPropertyInfos(path);
            return InternalReplace(propertyInfos, value, path);
        }

        public IPatchOperationBuilder<TItem> Set<TValue>(Expression<Func<TItem, TValue>> expression, TValue? value)
        {
            return InternalSet(expression, value);
        }

        public IPatchOperationBuilder<TItem> Set<TValue>(string path, TValue? value)
        {
            var propertyInfos = GetPropertyInfos(path);
            return InternalSet(propertyInfos, value, path);
        }

        public IPatchOperationBuilder<TItem> Add<TValue>(Expression<Func<TItem, TValue>> expression, TValue? value)
        {
            return InternalAdd(expression, value);
        }
        public IPatchOperationBuilder<TItem> Add<TValue>(string path, TValue? value)
        {
            var propertyInfos = GetPropertyInfos(path);
            return InternalAdd(propertyInfos, value, path);
        }

        public IPatchOperationBuilder<TItem> Remove<TValue>(Expression<Func<TItem, TValue>> expression)
        {
            return InternalRemove(expression);
        }

        public IPatchOperationBuilder<TItem> Remove(string path)
        {
            var propertyInfos = GetPropertyInfos(path);
            return InternalRemove(propertyInfos, path);
        }

        public IPatchOperationBuilder<TItem> Increment<TValue>(Expression<Func<TItem, TValue>> expression, double value)
        {
            return InternalIncrement(expression, value);
        }

        public IPatchOperationBuilder<TItem> Increment<TValue>(Expression<Func<TItem, TValue>> expression, long value)
        {
            return InternalIncrement(expression, value);
        }

        public IPatchOperationBuilder<TItem> Increment(string path, long value)
        {
            var propertyInfos = GetPropertyInfos(path);
            return InternalIncrement(propertyInfos, value, path);
        }

        public IPatchOperationBuilder<TItem> Increment(string path, double value)
        {
            var propertyInfos = GetPropertyInfos(path);
            return InternalIncrement(propertyInfos, value, path);
        }

        internal IPatchOperationBuilder<TItem> InternalReplace<TValue>(Expression<Func<TItem, TValue>> expression, TValue? value)
        {
            IReadOnlyList<PropertyInfo> propertyInfos = expression.GetPropertyInfos();
            return InternalReplace(propertyInfos, value);
        }

        internal IPatchOperationBuilder<TItem> InternalReplace<TValue>(IReadOnlyList<PropertyInfo> propertyInfos, TValue? value, string? path = null)
        {
            path = path ?? GetPropertyPath(propertyInfos);
            var index = ExtractIndexFromPath(path);

            path = NormalizePathPrefix(path);
            _rawPatchOperations.Add(new InternalPatchOperation(propertyInfos, value, PatchOperationType.Replace, index));
            _patchOperations.Add(PatchOperation.Replace(path, value));
            return this;
        }

        internal IPatchOperationBuilder<TItem> InternalSet<TValue>(Expression<Func<TItem, TValue>> expression, TValue? value, string? path = null)
        {
            IReadOnlyList<PropertyInfo> propertyInfos = expression.GetPropertyInfos();
            return InternalSet(propertyInfos, value, path);
        }

        internal IPatchOperationBuilder<TItem> InternalSet<TValue>(IReadOnlyList<PropertyInfo> propertyInfos, TValue? value, string? path = null)
        {
            path = path ?? GetPropertyPath(propertyInfos);
            var index = ExtractIndexFromPath(path);

            path = NormalizePathPrefix(path);
            _rawPatchOperations.Add(new InternalPatchOperation(propertyInfos, value, PatchOperationType.Set, index));
            _patchOperations.Add(PatchOperation.Set(path, value));
            return this;
        }

        internal IPatchOperationBuilder<TItem> InternalIncrement<TValue>(Expression<Func<TItem, TValue>> expression, long value, string? path = null)
        {
            IReadOnlyList<PropertyInfo> propertyInfos = expression.GetPropertyInfos();
            return InternalIncrement(propertyInfos, value, path);
        }

        internal IPatchOperationBuilder<TItem> InternalIncrement(IReadOnlyList<PropertyInfo> propertyInfos, long value, string? path = null)
        {
            path = path ?? GetPropertyPath(propertyInfos);
            var index = ExtractIndexFromPath(path);

            path = NormalizePathPrefix(path);
            _rawPatchOperations.Add(new InternalPatchOperation(propertyInfos, value, PatchOperationType.Increment, index));
            _patchOperations.Add(PatchOperation.Increment(path, value));
            return this;
        }

        internal IPatchOperationBuilder<TItem> InternalIncrement<TValue>(Expression<Func<TItem, TValue>> expression, double value, string? path = null)
        {
            IReadOnlyList<PropertyInfo> propertyInfos = expression.GetPropertyInfos();
            return InternalIncrement(propertyInfos, value, path);
        }

        internal IPatchOperationBuilder<TItem> InternalIncrement(IReadOnlyList<PropertyInfo> propertyInfos, double value, string? path = null)
        {
            path = path ?? GetPropertyPath(propertyInfos);
            var index = ExtractIndexFromPath(path);

            path = NormalizePathPrefix(path);
            _rawPatchOperations.Add(new InternalPatchOperation(propertyInfos, value, PatchOperationType.Increment, index));
            _patchOperations.Add(PatchOperation.Increment(path, value));
            return this;
        }

        internal IPatchOperationBuilder<TItem> InternalAdd<TValue>(Expression<Func<TItem, TValue>> expression, TValue? value, string? path = null)
        {
            IReadOnlyList<PropertyInfo> propertyInfos = expression.GetPropertyInfos();
            return InternalAdd(propertyInfos, value, path);
        }

        internal IPatchOperationBuilder<TItem> InternalAdd<TValue>(IReadOnlyList<PropertyInfo> propertyInfos, TValue? value, string? path = null)
        {
            path = path ?? GetPropertyPath(propertyInfos);
            var index = ExtractIndexFromPath(path);

            path = NormalizePathPrefix(path);
            path = NormalizePathSuffix(path, "/-");

            _rawPatchOperations.Add(new InternalPatchOperation(propertyInfos, value, PatchOperationType.Add, index));
            _patchOperations.Add(PatchOperation.Add(path, value));
            return this;
        }

        internal IPatchOperationBuilder<TItem> InternalRemove<TValue>(Expression<Func<TItem, TValue>> expression, string? path = null)
        {
            IReadOnlyList<PropertyInfo> propertyInfos = expression.GetPropertyInfos();
            return InternalRemove(propertyInfos, path);
        }

        internal IPatchOperationBuilder<TItem> InternalRemove(IReadOnlyList<PropertyInfo> propertyInfos, string? path = null)
        {
            path = path ?? GetPropertyPath(propertyInfos);

            path = NormalizePathPrefix(path);
            var index = ExtractIndexFromPath(path);
            _rawPatchOperations.Add(new InternalPatchOperation(propertyInfos, null, PatchOperationType.Remove, index));
            _patchOperations.Add(PatchOperation.Remove(path));
            return this;
        }

        private string NormalizePathPrefix(string path) => "/" + path.TrimStart('/').TrimEnd('/');

        private string NormalizePathSuffix(string path, string expectedSuffix) => path.EndsWith(expectedSuffix) ? path : path + expectedSuffix;

        private int? ExtractIndexFromPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            // Split the path by '/' to get the segments
            var segments = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            // Get the last segment
            var lastSegment = segments.LastOrDefault();

            if (string.IsNullOrEmpty(lastSegment))
            {
                return null;
            }

            // Check if the last segment is a number
            if (int.TryParse(lastSegment, out int position))
            {
                return position;
            }

            return null;
        }

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

        private string GetPropertyPath(IEnumerable<MemberInfo> propertyInfos)
        {
            List<string> propertiesNames = [];

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                JsonPropertyAttribute[] attributes =
                    propertyInfo.GetCustomAttributes<JsonPropertyAttribute>(true).ToArray();

                var propertyName = attributes.Length is 0
                    ? _namingStrategy.GetPropertyName(propertyInfo.Name, false)
                    : attributes[0].PropertyName;

                propertiesNames.Add(propertyName);
            }

            return string.Join("/", propertiesNames);
        }

        private string GetPropertyPath(MemberInfo propertyInfo) =>
            GetPropertyPath([propertyInfo]);

        public string GetPath<TValue>(Expression<Func<TItem, TValue>> expr)
        {
            var stack = new Stack<string>();
            Expression? expression = expr.Body;

            // Handle both MemberExpression and IndexExpression in the loop
            while (expression != null)
            {
                if (expression is MemberExpression memberExpression)
                {
                    expression = memberExpression.Expression;
                }
                else if (expression.NodeType == ExpressionType.ArrayIndex)
                {
                    // Handle array indexing, e.g., x.Tags[0] where Tags is an array
                    var binaryExpression = (BinaryExpression)expression;
                    var indexValue = Expression.Lambda(binaryExpression.Right).Compile().DynamicInvoke();
                    stack.Push($"{indexValue}");

                    // Move to the object being indexed (e.g., Tags)
                    expression = binaryExpression.Left;
                }
                else if (expression is MethodCallExpression methodCallExpression)
                {
                    // Handle list indexing like x.Tags[0], which results in a MethodCallExpression
                    if (methodCallExpression.Method.Name == "get_Item" && methodCallExpression.Arguments.Count == 1)
                    {
                        // Extract the index value
                        var indexValue = Expression.Lambda(methodCallExpression.Arguments[0]).Compile().DynamicInvoke();
                        stack.Push($"[{indexValue}]");

                        // Move to the object being indexed (e.g., Tags)
                        expression = methodCallExpression.Object;
                    }
                    else
                    {
                        throw new ArgumentException($"Expression '{expr}' refers to an unsupported method.");
                    }
                }
                else if (expression is UnaryExpression unaryExpression)
                {
                    // Handle conversions (e.g., Convert expressions)
                    expression = unaryExpression.Operand;
                }
                else
                {
                    break;
                }
            }

            var path = string.Join("/", stack);
            return NormalizePathPrefix(path);
        }

        /// <summary>
        /// Get the property name to replace.
        /// </summary>
        /// <remarks>If you're looking for nesting call the respective method with a given path instead.</remarks>
        /// <param name="path">The path to get the property.</param>
        /// <returns>Returns the path of the property name.</returns>
        internal IReadOnlyList<PropertyInfo> GetPropertyInfos(string path)
        {
            Type itemType = typeof(TItem);

            return GetNestedPropertyInfos(itemType, path);
        }

        private IReadOnlyList<PropertyInfo> GetNestedPropertyInfos(Type type, string path)
        {
            // Split the path by '/' to get the individual property names
            string[] properties = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            Type currentType = type;
            List<PropertyInfo> propertyInfos = [];
            PropertyInfo? currentPropertyInfo;

            // Iterate through each property in the path
            foreach (string propertyNameOrJsonProperty in properties)
            {

                //Checking if is a position in an array
                if (int.TryParse(propertyNameOrJsonProperty, out int pos) || propertyNameOrJsonProperty.EndsWith("-"))
                {
                    continue;
                }

                // Search for the property prioritizing JsonPropertyAttribute
                currentPropertyInfo = FindProperty(currentType, propertyNameOrJsonProperty);

                // If no matching property is found, return null
                if (currentPropertyInfo == null)
                {
                    throw new InvalidOperationException($"The property {path} does not exist on {type.Name}");
                }

                // Move to the next level of nesting
                currentType = currentPropertyInfo.PropertyType;
                propertyInfos.Add(currentPropertyInfo);

            }

            // Return the final PropertyInfo
            return propertyInfos;
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