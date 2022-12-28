// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Attributes;

namespace Microsoft.Azure.CosmosRepositoryAcceptanceTests.Models;

public class UniqueKeyPolicyItem : Item
{
    [UniqueKey(propertyPath: "/firstName")]
    public string FirstName { get; set; }

    [UniqueKey(propertyPath: "/age")]
    public int Age { get; set; }

    public string County { get; set; }

    [UniqueKey("colorKey", "/favouriteColor")]
    public string FavouriteColor { get; set; }

    protected override string GetPartitionKeyValue() => County;

    public UniqueKeyPolicyItem(string firstName, int age, string county, string favouriteColor)
    {
        FirstName = firstName;
        Age = age;
        County = county;
        FavouriteColor = favouriteColor;
    }
}