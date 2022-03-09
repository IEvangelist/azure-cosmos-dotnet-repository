// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using CleanArchitecture.Domain;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Attributes;

[Container("people")]
[PartitionKeyPath("/id")]
public class PersonItem : EtagItem
{
    public string Name { get; set; }

    public int Age { get; set; }

    public override string ToString() =>
        $"Person Item (Name = {Name}, Age = {Age}, Id = {Id})";
}



static class PersonExtensions
{
    public static PersonEntity ToEntity(this PersonItem personItem) =>
        new (personItem.Name, personItem.Age, personItem.Id);

    public static PersonItem ToItem(this PersonEntity personItem) =>
        new ()
        {
            Name = personItem.Name,
            Age = personItem.Age,
            Id = personItem.Id
        };
}