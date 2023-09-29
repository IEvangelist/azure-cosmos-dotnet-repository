// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Providers;

/// <inheritdoc />
class DefaultCosmosUniqueKeyPolicyProvider : ICosmosUniqueKeyPolicyProvider
{
    static readonly Type s_attributeType = typeof(UniqueKeyAttribute);

    /// <inheritdoc />
    public UniqueKeyPolicy? GetUniqueKeyPolicy<TItem>() where TItem : IItem =>
        GetUniqueKeyPolicy(typeof(TItem));

    public UniqueKeyPolicy? GetUniqueKeyPolicy(Type itemType)
    {
        Dictionary<string, List<string>> keyNameToPathsMap = [];

        foreach ((UniqueKeyAttribute? uniqueKey, var propertyName) in itemType.GetProperties()
                     .Where(x => Attribute.IsDefined(x, s_attributeType))
                     .Select(x => (x.GetCustomAttribute<UniqueKeyAttribute>(), x.Name)))
        {
            if (uniqueKey is null) continue;

            var propertyValue = (uniqueKey.PropertyPath ?? $"/{propertyName ?? ""}")!;

            if (keyNameToPathsMap.TryGetValue(uniqueKey.KeyName, out List<string>? value)
                && value is not null)
            {
                value.Add(propertyValue);
                continue;
            }

            keyNameToPathsMap[uniqueKey.KeyName] = [propertyValue];
        }

        if (!keyNameToPathsMap.Any())
        {
            return null;
        }

        UniqueKeyPolicy policy = new();

        foreach (KeyValuePair<string, List<string>> keyNameToPaths in keyNameToPathsMap)
        {
            UniqueKey key = new();

            foreach (var path in keyNameToPaths.Value)
            {
                key.Paths.Add(path);
            }

            policy.UniqueKeys.Add(key);
        }

        return policy;
    }
}