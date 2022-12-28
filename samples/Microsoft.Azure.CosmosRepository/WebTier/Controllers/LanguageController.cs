// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CosmosRepository;
using WebTier.Models;

namespace WebTier.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LanguageController : ControllerBase
{
    private readonly IRepository<Language> _repository;

    public LanguageController(IRepositoryFactory factory) =>
        _repository = factory.RepositoryOf<Language>();

    [HttpGet(Name = nameof(GetLanguages))]
    public ValueTask<IEnumerable<Language>> GetLanguages() =>
        _repository.GetAsync(l => l.InitialReleaseDate > new DateTime(1984, 7, 7));

    [HttpGet("{id}", Name = nameof(GetLanguageById))]
    public ValueTask<Language> GetLanguageById(string id) =>
        _repository.GetAsync(id);

    [HttpPost(Name = nameof(PostLanguages))]
    public ValueTask<IEnumerable<Language>> PostLanguages([FromBody] params Language[] languages) =>
        _repository.CreateAsync(languages);

    [HttpPut(Name = nameof(PutLanguage))]
    public ValueTask<Language> PutLanguage([FromBody] Language language) =>
        _repository.UpdateAsync(language);

    [HttpPut("{id}")]
    public async ValueTask<IActionResult> UpdateName([FromRoute] string id, [FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest("A name is required");
        }

        await _repository
            .UpdateAsync(id: id,
                builder: builder => builder.Replace(language => language.Name, name));

        return Ok();
    }

    [HttpDelete("{id}", Name = nameof(DeleteLanguage))]
    public ValueTask DeleteLanguage(string id) =>
        _repository.DeleteAsync(id);
}