// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ServiceTier
{
    public interface IExampleService
    {
        ValueTask<Person> AddPersonAsync(Person person);
        Task<Person[]> AddPeopleAsync(IEnumerable<Person> people);

        ValueTask<Person> ReadPersonByIdAsync(string id);
        ValueTask<IEnumerable<Person>> ReadPeopleAsync(Expression<Func<Person, bool>> matches);

        ValueTask<Person> UpdatePersonAsync(Person person);

        ValueTask<Person> DeletePersonAsync(Person person);
    }
}