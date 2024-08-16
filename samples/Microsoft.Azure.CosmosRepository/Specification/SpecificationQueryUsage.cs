// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Specification;

namespace Specification;

public class SpecificationFilterSamples(IRepository<Person> repository)
{
    public async Task FilterSamples()
    {
        AllPersonsWithNameSpecification nameSpecification = new("Tom");
        IQueryResult<Person> personsWithNameTom = await repository.QueryAsync(nameSpecification);

        Console.WriteLine($"Found {personsWithNameTom.Items.Count} with name Tom");
        Console.WriteLine($"Change for query {personsWithNameTom.Charge}");

        var age = 25;
        AllPersonsOlderThanSpecification ageSpecification = new(age);
        IQueryResult<Person> peopleOlderThan25 = await repository.QueryAsync(ageSpecification);

        Console.WriteLine($"Found {peopleOlderThan25.Items.Count} people older than {age}");
        Console.WriteLine($"Change for query {peopleOlderThan25.Charge}");
    }

    private class AllPersonsWithNameSpecification : DefaultSpecification<Person>
    {
        public AllPersonsWithNameSpecification(string name)
        {
            Query.Where(p => p.Name == name);
        }
    }

    private class AllPersonsOlderThanSpecification : DefaultSpecification<Person>
    {
        public AllPersonsOlderThanSpecification(int age)
        {
            Query.Where(p => p.Age > age);
        }
    }
}
