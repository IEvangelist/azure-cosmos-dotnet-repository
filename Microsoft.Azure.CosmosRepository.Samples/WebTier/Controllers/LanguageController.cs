// Copyright (c) IEvangelist. All rights reserved. Licensed under the MIT License.

namespace WebTier.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.CosmosRepository;
    using WebTier.Models;

    [ApiController]
    [Route("api/[controller]")]
    public class LanguageController : ControllerBase
    {
        private readonly IRepository<Language> repository;

        public LanguageController(IRepositoryFactory factory) => this.repository = factory.RepositoryOf<Language>();

        [HttpDelete("{id}", Name = nameof(DeleteLanguage))]
        public ValueTask DeleteLanguage(string id) => this.repository.DeleteAsync(id);

        [HttpGet("{id}", Name = nameof(GetLanguageById))]
        public ValueTask<Language> GetLanguageById(string id) => this.repository.GetAsync(id);

        [HttpGet(Name = nameof(GetLanguages))]
        public ValueTask<IEnumerable<Language>> GetLanguages() =>
            this.repository.GetAsync(l => l.InitialReleaseDate > new DateTime(1984, 7, 7));

        [HttpPost(Name = nameof(PostLanguages))]
        public ValueTask<IEnumerable<Language>> PostLanguages([FromBody] params Language[] languages) =>
            this.repository.CreateAsync(languages);

        [HttpPut(Name = nameof(PutLanguage))]
        public ValueTask<Language> PutLanguage([FromBody] Language language) => this.repository.UpdateAsync(language);
    }
}
