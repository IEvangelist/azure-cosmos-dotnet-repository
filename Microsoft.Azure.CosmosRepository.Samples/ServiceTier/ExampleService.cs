// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository;

namespace ServiceTier
{
    public class ExampleService : IExampleService
    {
        readonly IRepository<Person> _personRepository;

        public ExampleService(IRepository<Person> personRepository) =>
            _personRepository = personRepository;

        public Task<Person[]> AddPeopleAsync(IEnumerable<Person> people) =>
            _personRepository.CreateAsync(people);

        public ValueTask<Person> AddPersonAsync(Person person) =>
            _personRepository.CreateAsync(person);

        public ValueTask<Person> DeletePersonAsync(Person person) =>
            _personRepository.DeleteAsync(person);

        public ValueTask<IEnumerable<Person>> ReadPeopleAsync(Func<Person, bool> matches) =>
            _personRepository.GetAsync(p => matches(p));

        public ValueTask<Person> ReadPersonByIdAsync(string id) =>
            _personRepository.GetAsync(id);

        public ValueTask<Person> UpdatePersonAsync(Person person) =>
            _personRepository.UpdateAsync(person);
    }
}
