// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using CleanArchitecture.Domain;
using Microsoft.Azure.CosmosRepository.CleanArchitecture;

namespace CleanArchitecture;

public class PersonRepository : IPersonRepository
{
    private readonly IEtagMappedRepository<PersonItem, PersonEntity> _mappedRepository;

    public PersonRepository(IEtagMappedRepository<PersonItem, PersonEntity> mappedRepository)
    {
        _mappedRepository = mappedRepository;
    }

    public void Dispose()
    {
        _mappedRepository.Dispose();
    }

    public async ValueTask<PersonEntity> GetPerson(string id) =>
        await _mappedRepository.GetAsync(id);

    public async ValueTask<PersonEntity> UpdatePerson(PersonEntity person) =>
        await _mappedRepository.UpdateAsync(person);
}