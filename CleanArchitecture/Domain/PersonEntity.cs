// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace CleanArchitecture.Domain;

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

    public override string ToString() =>
        $"Person Entity (Name = {Name}, Age = {Age}, Id = {Id})";

    public void Birthday()
    {
        Age++;
    }
}