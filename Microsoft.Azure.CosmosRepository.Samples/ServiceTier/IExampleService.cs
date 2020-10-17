// Copyright (c) IEvangelist. All rights reserved. Licensed under the MIT License.

namespace ServiceTier
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IExampleService
    {
        ValueTask<IEnumerable<Person>> AddPeopleAsync(IEnumerable<Person> people);

        ValueTask<Person> AddPersonAsync(Person person);

        ValueTask DeletePersonAsync(Person person);

        ValueTask<IEnumerable<Person>> ReadPeopleAsync(Expression<Func<Person, bool>> matches);

        ValueTask<Person?> ReadPersonByIdAsync(string id, string partitionKey);

        ValueTask<Person> UpdatePersonAsync(Person person);
    }
}
