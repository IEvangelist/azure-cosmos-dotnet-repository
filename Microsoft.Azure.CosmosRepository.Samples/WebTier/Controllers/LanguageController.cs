using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CosmosRepository;
using WebTier.Models;

namespace WebTier.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LanguageController : ControllerBase
    {
        private readonly IRepository<Language> _repository;

        public LanguageController(IRepository<Language> repository) =>
            _repository = repository;

        [HttpGet]
        public ValueTask<IEnumerable<Language>> Get() =>
            _repository.GetAsync(l => l.InitialReleaseDate > new DateTime(1984, 7, 7));

        [HttpGet("{id}")]
        public ValueTask<Language> Get(string id) =>
            _repository.GetAsync(id);

        [HttpPost]
        public ValueTask<Language> Post([FromBody] Language language) =>
            _repository.CreateAsync(language);

        [HttpPut]
        public ValueTask<Language> Put([FromBody] Language language) =>
            _repository.UpdateAsync(language);

        [HttpDelete("{id}")]
        public ValueTask<Language> Delete(string id) =>
            _repository.DeleteAsync(id);
    }
}
