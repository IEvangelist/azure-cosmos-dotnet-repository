// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Attributes;

namespace Microsoft.Azure.CosmosRepositoryAcceptanceTests.Models;

public class UniqueKeyPolicyItem : Item
{

    [UniqueKey("firstName")]
    public string FirstName { get; set; }

    [UniqueKey("age")]
    public int Age { get; set; }

    public UniqueKeyPolicyItem(string firstName, int age)
    {
        FirstName = firstName;
        Age = age;
    }
}