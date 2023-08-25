// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Linq.Expressions;
using Microsoft.Azure.CosmosRepository;

namespace ServiceTier;

public class ExampleService : IExampleService
{
    readonly IRepository<Person> _personRepository;

    public ExampleService(IRepository<Person> personRepository) =>
        _personRepository = personRepository;

    public ValueTask<IEnumerable<Person>> AddPeopleAsync(IEnumerable<Person> people) =>
        _personRepository.CreateAsync(people);

    public ValueTask<Person> AddPersonAsync(Person person) =>
        _personRepository.CreateAsync(person);

    public ValueTask DeletePersonAsync(Person person) =>
        _personRepository.DeleteAsync(person);

    public ValueTask<IEnumerable<Person>> ReadPeopleAsync(Expression<Func<Person, bool>> matches) =>
        _personRepository.GetAsync(matches);

    public ValueTask<Person> ReadPersonByIdAsync(string id, string partitionKey) =>
        _personRepository.GetAsync(id, partitionKey);

    public ValueTask<Person> UpdatePersonAsync(Person person) =>
        _personRepository.UpdateAsync(person);
}
