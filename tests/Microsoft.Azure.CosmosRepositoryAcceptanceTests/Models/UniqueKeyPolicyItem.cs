// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository.Attributes;

namespace Microsoft.Azure.CosmosRepositoryAcceptanceTests.Models;

public class UniqueKeyPolicyItem(string firstName, int age, string county, string favouriteColor) : Item
{
    [UniqueKey(propertyPath: "/firstName")]
    public string FirstName { get; set; } = firstName;

    [UniqueKey(propertyPath: "/age")]
    public int Age { get; set; } = age;

    public string County { get; set; } = county;

    [UniqueKey("colorKey", "/favouriteColor")]
    public string FavouriteColor { get; set; } = favouriteColor;

    protected override string GetPartitionKeyValue() => County;
}