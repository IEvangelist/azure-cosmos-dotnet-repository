// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Linq.Expressions;

namespace ServiceTier;

public interface IExampleService
{
    ValueTask<Person> AddPersonAsync(Person person);
    ValueTask<IEnumerable<Person>> AddPeopleAsync(IEnumerable<Person> people);

    ValueTask<Person> ReadPersonByIdAsync(string id, string partitionKey);
    ValueTask<IEnumerable<Person>> ReadPeopleAsync(Expression<Func<Person, bool>> matches);

    ValueTask<Person> UpdatePersonAsync(Person person);

    ValueTask DeletePersonAsync(Person person);
}