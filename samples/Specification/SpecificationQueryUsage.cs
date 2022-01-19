// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Specification;

namespace Specification;

public class SpecificationFilterSamples
{
    private readonly IRepository<Person> _repository;

    public SpecificationFilterSamples(IRepository<Person> repository)
    {
        _repository = repository;
    }

    public async Task FilterSamples()
    {
        AllPersonsWithNameSpecification nameSpecification = new("Tom");
        IQueryResult<Person> personsWithNameTom = await _repository.GetAsync(nameSpecification);

        Console.WriteLine($"Found {personsWithNameTom.Items.Count} with name Tom");
        Console.WriteLine($"Change for query {personsWithNameTom.Charge}");

        int age = 25;
        AllPersonsOlderThanSpecifciation ageSpecification = new(age);
        IQueryResult<Person> peopleOlderThan25 = await _repository.GetAsync(ageSpecification);

        Console.WriteLine($"Found {peopleOlderThan25.Items.Count} people older than {age}");
        Console.WriteLine($"Change for query {peopleOlderThan25.Charge}");
    }

    private class AllPersonsWithNameSpecification : ListSpecification<Person>
    {
        public AllPersonsWithNameSpecification(string name)
        {
            Query.Where(p => p.Name == name);
        }
    }

    private class AllPersonsOlderThanSpecifciation : ListSpecification<Person>
    {
        public AllPersonsOlderThanSpecifciation(int age)
        {
            Query.Where(p => p.Age > age);
        }
    }
}
