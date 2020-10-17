// Copyright (c) IEvangelist. All rights reserved. Licensed under the MIT License.

namespace ServiceTier
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Microsoft.Azure.CosmosRepository;

    public class ExampleService : IExampleService
    {
        private readonly IRepository<Person> personRepository;

        public ExampleService(IRepository<Person> personRepository) =>
            this.personRepository = personRepository;

        public ValueTask<IEnumerable<Person>> AddPeopleAsync(IEnumerable<Person> people) =>
            this.personRepository.CreateAsync(people);

        public ValueTask<Person> AddPersonAsync(Person person) =>
            this.personRepository.CreateAsync(person);

        public ValueTask DeletePersonAsync(Person person) =>
            this.personRepository.DeleteAsync(person);

        public ValueTask<IEnumerable<Person>> ReadPeopleAsync(Expression<Func<Person, bool>> matches) =>
            this.personRepository.GetAsync(matches);

        public ValueTask<Person?> ReadPersonByIdAsync(string id, string partitionKey) =>
            this.personRepository.GetAsync(id, partitionKey);

        public ValueTask<Person> UpdatePersonAsync(Person person) =>
            this.personRepository.UpdateAsync(person);
    }
}
