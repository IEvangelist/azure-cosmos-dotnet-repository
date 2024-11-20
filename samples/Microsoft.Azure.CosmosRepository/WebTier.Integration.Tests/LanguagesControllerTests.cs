// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Extensions.DependencyInjection;
using WebTier.Integration.Tests.Factories;
using WebTier.Models;
using Xunit;

namespace WebTier.Integration.Tests;

public class LanguagesControllerTests : IClassFixture<WebTierApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly IRepositoryFactory _repositoryFactory;

    public LanguagesControllerTests(WebTierApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _repositoryFactory = factory.Services.GetRequiredService<IRepositoryFactory>();
    }

    [Fact]
    public async Task Post_Always_Creates_A_Language()
    {
        //Arrange
        LanguageDto language = new()
        {
            Id = string.Empty,
            Name = "C#",
            Description = "A language created by Microsoft.",
            Aliases = ["C#", ".NET"],
            PrimaryStyle = ProgrammingStyle.ObjectOriented,
            InitialReleaseDate = new DateTime(2001, 10, 25)
        };

        List<LanguageDto> languages =
        [
            language
        ];

        IRepository<Language> repository = _repositoryFactory.RepositoryOf<Language>();

        //Act
        HttpResponseMessage response = await _client.PostAsJsonAsync("api/language", languages);

        //Assert
        response.EnsureSuccessStatusCode();
        List<LanguageDto> responseLanguages = await response.Content.ReadFromJsonAsync<List<LanguageDto>>(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        });
        Assert.NotNull(responseLanguages);
        LanguageDto responseLanguage = responseLanguages.First();
        Language databaseLanguage = await repository.GetAsync(responseLanguage.Id);

        Assert.Equal(language.Name, databaseLanguage.Name);
        Assert.Equal(language.Description, databaseLanguage.Description);
        Assert.Equal(language.PrimaryStyle, databaseLanguage.PrimaryStyle);
    }
}