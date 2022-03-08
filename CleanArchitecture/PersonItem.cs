// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Attributes;

[Container("people")]
[PartitionKeyPath("/id")]
public class PersonItem : EtagItem
{
    public string Name { get; set; }

    public int Age { get; set; }

    public override string ToString() =>
        $"Person Item (Name = {Name}, Age = {Age})";
}

public class PersonEntity
{
    public PersonEntity(string name, int age)
    {
        Name = name;
        Age = age;
    }

    public string Name { get; private set; }

    public int Age { get; private set; }

    public override string ToString() =>
        $"Person Entity (Name = {Name}, Age = {Age})";

    public void Birthday()
    {
        Age++;
    }
}

static class PersonExtensions
{
    public static PersonEntity ToEntity(this PersonItem personItem) =>
        new (personItem.Name, personItem.Age);

    public static PersonItem ToItem(this PersonEntity personItem) =>
        new ()
        {
            Name = personItem.Name,
            Age = personItem.Age
        };
}