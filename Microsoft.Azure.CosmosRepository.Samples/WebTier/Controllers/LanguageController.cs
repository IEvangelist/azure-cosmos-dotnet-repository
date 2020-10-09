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

        [HttpGet(Name = nameof(GetLanguages))]
        public ValueTask<IEnumerable<Language>> GetLanguages() =>
            _repository.GetAsync(l => l.InitialReleaseDate > new DateTime(1984, 7, 7));

        [HttpGet("{id}", Name = nameof(GetLanguageById))]
        public ValueTask<Language> GetLanguageById(string id) =>
            _repository.GetAsync(id);

        [HttpPost(Name = nameof(PostLanguages))]
        public Task<Language[]> PostLanguages([FromBody] params Language[] languages) =>
            _repository.CreateAsync(languages);

        [HttpPut(Name = nameof(PutLanguage))]
        public ValueTask<Language> PutLanguage([FromBody] Language language) =>
            _repository.UpdateAsync(language);

        [HttpDelete("{id}", Name = nameof(DeleteLanguage))]
        public ValueTask DeleteLanguage(string id) =>
            _repository.DeleteAsync(id);
    }
}
