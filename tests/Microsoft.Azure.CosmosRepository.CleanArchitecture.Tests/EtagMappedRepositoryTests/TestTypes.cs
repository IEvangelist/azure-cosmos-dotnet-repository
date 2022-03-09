// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.Azure.CosmosRepository.CleanArchitecture.Tests.EtagMappedRepositoryTests;

public class TestTypes
{
    public class PersonItem : EtagItem
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class PersonEntity
    {
        public PersonEntity(string name, int age, string? id = null)
        {
            Id = id ?? Guid.NewGuid().ToString();
            Name = name;
            Age = age;
        }

        public string Id { get; private set; }
        public string Name { get; private set; }
        public int Age { get; private set; }
    }
}