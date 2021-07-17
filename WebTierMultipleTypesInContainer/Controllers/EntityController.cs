using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository;
using WebApplication2;
using WebTierMultipleTypesInContainer.Models;

namespace WebTierMultipleTypesInContainer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EntityController : ControllerBase
    {
        private readonly IRepository<Person> _repository;
        private readonly IRepository<Animal> _animalRepository;

        public EntityController(IRepository<Person> repository, IRepository<Animal> animalRepository)
        {
            _repository = repository;
            _animalRepository = animalRepository;
        }

        [HttpPost(Name = nameof(PostEntities))]
        public async Task PostEntities()
        {
            await _repository.CreateAsync(new Person
            {
                Name = "SameName"
            });

            await _animalRepository.CreateAsync(new Animal
            {
                Name = "SameName"
            });
        }

        [HttpGet("GetPerson")]
        public async ValueTask<IEnumerable<Person>> GetPeople()
        {
            return (await _repository.GetAsync(p => p.Name == "SameName")).ToArray();
        }

        [HttpGet("GetAnimal")]
        public async ValueTask<IEnumerable<Animal>> GetAnimals()
        {
            return (await _animalRepository.GetAsync(p => p.Name == "SameName")).ToArray();
        }

        [HttpDelete("DeletePerson")]
        public async ValueTask DeletePerson(string id) => await _repository.DeleteAsync(id);

        [HttpDelete("DeleteAnimal")]
        public async ValueTask DeleteAnimal(string id) => await _animalRepository.DeleteAsync(id);

        [HttpPut("{id}", Name = nameof(UpdateName))]
        public async ValueTask UpdateName(string id, [FromBody] UpdateRequest request)
        {
            if (request.EntityType == EntityType.Person)
            {
                Person person = await _repository.GetAsync(id);
                person.Name = request.Name;
                await _repository.UpdateAsync(person);
            }
            else
            {
                Animal animal = await _animalRepository.GetAsync(id);
                animal.Name = request.Name;
                await _animalRepository.UpdateAsync(animal);
            }
        }
    }

    public enum EntityType
    {
        Person,
        Animal
    }

    public class UpdateRequest
    {
        public string Name { get; set; }
        public EntityType EntityType { get; set; }
    }

}
