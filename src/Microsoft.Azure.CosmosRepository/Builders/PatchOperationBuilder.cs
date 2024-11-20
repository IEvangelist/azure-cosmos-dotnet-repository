// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Builders;

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
        IReadOnlyList<PropertyInfo> propertyInfos = expression.GetPropertyInfos();
        var propertyToReplace = GetPropertyToReplace(propertyInfos);

        _rawPatchOperations.Add(new InternalPatchOperation(propertyInfos, value, PatchOperationType.Replace));
        _patchOperations.Add(PatchOperation.Replace($"/{propertyToReplace}", value));

        return this;
    }

    private string GetPropertyToReplace(IEnumerable<MemberInfo> propertyInfos)
    {
        List<string> propertiesNames = [];

        foreach (PropertyInfo propertyInfo in propertyInfos.Cast<PropertyInfo>())
        {
            JsonPropertyAttribute[] attributes =
                propertyInfo.GetCustomAttributes<JsonPropertyAttribute>(true).ToArray();

            var propertyName = attributes.Length is 0
                ? _namingStrategy.GetPropertyName(propertyInfo.Name, false)
                : attributes[0].PropertyName ?? propertyInfo.Name;

            propertiesNames.Add(propertyName);
        }

        return string.Join("/", propertiesNames);
    }
}