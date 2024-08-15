// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

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
            string propertyToReplace = GetPropertyToReplace(expression);
            return Replace(propertyToReplace, value);
        }
        public IPatchOperationBuilder<TItem> Replace<TValue>(string path, TValue value)
        {
            _patchOperations.Add(PatchOperation.Replace($"/{path}", value));
            return this;
        }

        public IPatchOperationBuilder<TItem> Set<TValue>(Expression<Func<TItem, TValue>> expression, TValue? value)
        {
            string propertyToReplace = GetPropertyToReplace(expression);
            return Set(propertyToReplace, value);
        }

        public IPatchOperationBuilder<TItem> Set<TValue>(string path, TValue? value)
        {
            _patchOperations.Add(PatchOperation.Set($"/{path}", value));
            return this;
        }

        public IPatchOperationBuilder<TItem> Add<TValue>(Expression<Func<TItem, TValue>> expression, TValue? value)
        {
            string propertyToReplace = GetPropertyToReplace(expression);
            return Add(propertyToReplace, value);
        }

        public IPatchOperationBuilder<TItem> Add<TValue>(string path, TValue? value)
        {
            _patchOperations.Add(PatchOperation.Add($"/{path}", value));
            return this;
        }

        public IPatchOperationBuilder<TItem> Remove<TValue>(Expression<Func<TItem, TValue>> expression)
        {
            string propertyToReplace = GetPropertyToReplace(expression);
            return Remove(propertyToReplace);
        }

        public IPatchOperationBuilder<TItem> Remove(string path)
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

        public IPatchOperationBuilder<TItem> Increment<TValue>(Expression<Func<TItem, TValue>> expression, double value)
        {
            var propertyToReplace = GetPropertyToReplace(expression);
            return Increment(propertyToReplace, value);
        }

        public IPatchOperationBuilder<TItem> Increment<TValue>(Expression<Func<TItem, TValue>> expression, long value)
        {
            var propertyToReplace = GetPropertyToReplace(expression);
            return Increment(propertyToReplace, value);
        }

        /// <summary>
        /// Get the property name to replace. This only works for a single level of nesting.
        /// </summary>
        /// <remarks>If you're looking for nesting call the respective method with a given path instead.</remarks>
        /// <param name="expression">The property to get the path of.</param>
        /// <returns>Returns the path of the property name.</returns>
        internal string GetPropertyToReplace<TValue>(Expression<Func<TItem, TValue>> expression)
        {
            PropertyInfo property = expression.GetPropertyInfo();

            JsonPropertyAttribute[] attributes =
                property.GetCustomAttributes<JsonPropertyAttribute>(true).ToArray();

            return attributes.Length is 0
                ? _namingStrategy.GetPropertyName(property.Name, false)
                : attributes[0].PropertyName;
        }

    }
}