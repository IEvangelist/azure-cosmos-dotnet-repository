<!-- markdownlint-capture -->
<!-- markdownlint-disable -->

# Code Metrics

This file is dynamically maintained by a bot, *please do not* edit this by hand. It represents various [code metrics](https://aka.ms/dotnet/code-metrics), such as cyclomatic complexity, maintainability index, and so on.

<div id='azurefunctiontier'></div>

## AzureFunctionTier :heavy_check_mark:

The *AzureFunctionTier.csproj* project file contains:

- 2 namespaces.
- 4 named types.
- 117 total lines of source code.
- Approximately 35 lines of executable code.
- The highest cyclomatic complexity is 4 :heavy_check_mark:.

<details>
<summary>
  <strong id="azurefunctiontier">
    AzureFunctionTier :heavy_check_mark:
  </strong>
</summary>
<br>

The `AzureFunctionTier` namespace contains 2 named types.

- 2 named types.
- 67 total lines of source code.
- Approximately 18 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

<details>
<summary>
  <strong id="startup">
    Startup :heavy_check_mark:
  </strong>
</summary>
<br>

- The `Startup` contains 1 members.
- 5 total lines of source code.
- Approximately 1 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [9](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/AzureFunctionTier/Startup.cs#L9 "void Startup.Configure(IFunctionsHostBuilder builder)") | 100 | 1 :heavy_check_mark: | 0 | 2 | 2 / 1 |

<a href="#azurefunctiontier">:top: back to AzureFunctionTier</a>

</details>

<details>
<summary>
  <strong id="usersapi">
    UsersApi :heavy_check_mark:
  </strong>
</summary>
<br>

- The `UsersApi` contains 4 members.
- 56 total lines of source code.
- Approximately 17 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [18](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/AzureFunctionTier/UsersApi.cs#L18 "IRepository<User> UsersApi._repository") | 100 | 0 :heavy_check_mark: | 0 | 2 | 1 / 0 |
| Method | [20](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/AzureFunctionTier/UsersApi.cs#L20 "UsersApi.UsersApi(IRepositoryFactory factory)") | 96 | 1 :heavy_check_mark: | 0 | 4 | 1 / 1 |
| Method | [24](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/AzureFunctionTier/UsersApi.cs#L24 "Task<IActionResult> UsersApi.GetUsers(HttpRequest req, ILogger log, CancellationToken hostCancellationToken)") | 65 | 2 :heavy_check_mark: | 0 | 14 | 23 / 9 |
| Method | [54](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/AzureFunctionTier/UsersApi.cs#L54 "Task<IActionResult> UsersApi.PostUser(HttpRequest req, ILogger log, CancellationToken hostCancellationToken)") | 68 | 1 :heavy_check_mark: | 0 | 14 | 25 / 7 |

<a href="#azurefunctiontier">:top: back to AzureFunctionTier</a>

</details>

</details>

<details>
<summary>
  <strong id="azurefunctiontier-model">
    AzureFunctionTier.Model :heavy_check_mark:
  </strong>
</summary>
<br>

The `AzureFunctionTier.Model` namespace contains 2 named types.

- 2 named types.
- 50 total lines of source code.
- Approximately 17 lines of executable code.
- The highest cyclomatic complexity is 4 :heavy_check_mark:.

<details>
<summary>
  <strong id="postuserrequest">
    PostUserRequest :heavy_check_mark:
  </strong>
</summary>
<br>

- The `PostUserRequest` contains 4 members.
- 27 total lines of source code.
- Approximately 7 lines of executable code.
- The highest cyclomatic complexity is 4 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Property | [14](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/AzureFunctionTier/Model/PostUserRequest.cs#L14 "string PostUserRequest.EmailAddress") | 100 | 2 :heavy_check_mark: | 0 | 1 | 2 / 2 |
| Property | [8](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/AzureFunctionTier/Model/PostUserRequest.cs#L8 "string PostUserRequest.FirstName") | 100 | 2 :heavy_check_mark: | 0 | 1 | 2 / 2 |
| Property | [11](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/AzureFunctionTier/Model/PostUserRequest.cs#L11 "string PostUserRequest.LastName") | 100 | 2 :heavy_check_mark: | 0 | 1 | 2 / 2 |
| Method | [22](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/AzureFunctionTier/Model/PostUserRequest.cs#L22 "PostUserRequest.implicit operator User(PostUserRequest postUserRequest)") | 87 | 4 :heavy_check_mark: | 0 | 2 | 13 / 1 |

<a href="#azurefunctiontier-model">:top: back to AzureFunctionTier.Model</a>

</details>

<details>
<summary>
  <strong id="user">
    User :heavy_check_mark:
  </strong>
</summary>
<br>

- The `User` contains 5 members.
- 17 total lines of source code.
- Approximately 10 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Property | [21](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/AzureFunctionTier/Model/User.cs#L21 "string User.EmailAddress") | 100 | 2 :heavy_check_mark: | 0 | 1 | 2 / 2 |
| Property | [12](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/AzureFunctionTier/Model/User.cs#L12 "string User.FirstName") | 100 | 2 :heavy_check_mark: | 0 | 1 | 2 / 2 |
| Property | [18](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/AzureFunctionTier/Model/User.cs#L18 "string User.FullName") | 95 | 2 :heavy_check_mark: | 0 | 1 | 2 / 2 |
| Property | [15](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/AzureFunctionTier/Model/User.cs#L15 "string User.LastName") | 100 | 2 :heavy_check_mark: | 0 | 1 | 2 / 2 |
| Property | [9](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/AzureFunctionTier/Model/User.cs#L9 "string User.Nickname") | 100 | 2 :heavy_check_mark: | 0 | 1 | 2 / 2 |

<a href="#azurefunctiontier-model">:top: back to AzureFunctionTier.Model</a>

</details>

</details>

<a href="#azurefunctiontier">:top: back to AzureFunctionTier</a>

<div id='servicetier'></div>

## ServiceTier :heavy_check_mark:

The *ServiceTier.csproj* project file contains:

- 1 namespaces.
- 5 named types.
- 295 total lines of source code.
- Approximately 107 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

<details>
<summary>
  <strong id="servicetier">
    ServiceTier :heavy_check_mark:
  </strong>
</summary>
<br>

The `ServiceTier` namespace contains 5 named types.

- 5 named types.
- 295 total lines of source code.
- Approximately 107 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

<details>
<summary>
  <strong id="exampleservice">
    ExampleService :heavy_check_mark:
  </strong>
</summary>
<br>

- The `ExampleService` contains 8 members.
- 25 total lines of source code.
- Approximately 7 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [14](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/ExampleService.cs#L14 "IRepository<Person> ExampleService._personRepository") | 100 | 0 :heavy_check_mark: | 0 | 2 | 1 / 0 |
| Method | [16](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/ExampleService.cs#L16 "ExampleService.ExampleService(IRepository<Person> personRepository)") | 96 | 1 :heavy_check_mark: | 0 | 2 | 2 / 1 |
| Method | [19](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/ExampleService.cs#L19 "ValueTask<IEnumerable<Person>> ExampleService.AddPeopleAsync(IEnumerable<Person> people)") | 97 | 1 :heavy_check_mark: | 0 | 5 | 2 / 1 |
| Method | [22](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/ExampleService.cs#L22 "ValueTask<Person> ExampleService.AddPersonAsync(Person person)") | 97 | 1 :heavy_check_mark: | 0 | 4 | 2 / 1 |
| Method | [25](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/ExampleService.cs#L25 "ValueTask ExampleService.DeletePersonAsync(Person person)") | 97 | 1 :heavy_check_mark: | 0 | 4 | 2 / 1 |
| Method | [28](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/ExampleService.cs#L28 "ValueTask<IEnumerable<Person>> ExampleService.ReadPeopleAsync(Expression<Func<Person, bool>> matches)") | 97 | 1 :heavy_check_mark: | 0 | 7 | 2 / 1 |
| Method | [31](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/ExampleService.cs#L31 "ValueTask<Person> ExampleService.ReadPersonByIdAsync(string id, string partitionKey)") | 94 | 1 :heavy_check_mark: | 0 | 4 | 2 / 1 |
| Method | [34](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/ExampleService.cs#L34 "ValueTask<Person> ExampleService.UpdatePersonAsync(Person person)") | 97 | 1 :heavy_check_mark: | 0 | 4 | 2 / 1 |

<a href="#servicetier">:top: back to ServiceTier</a>

</details>

<details>
<summary>
  <strong id="iexampleservice">
    IExampleService :heavy_check_mark:
  </strong>
</summary>
<br>

- The `IExampleService` contains 6 members.
- 12 total lines of source code.
- Approximately 0 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [14](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/IExampleService.cs#L14 "ValueTask<IEnumerable<Person>> IExampleService.AddPeopleAsync(IEnumerable<Person> people)") | 100 | 1 :heavy_check_mark: | 0 | 3 | 1 / 0 |
| Method | [13](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/IExampleService.cs#L13 "ValueTask<Person> IExampleService.AddPersonAsync(Person person)") | 100 | 1 :heavy_check_mark: | 0 | 2 | 1 / 0 |
| Method | [21](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/IExampleService.cs#L21 "ValueTask IExampleService.DeletePersonAsync(Person person)") | 100 | 1 :heavy_check_mark: | 0 | 2 | 1 / 0 |
| Method | [17](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/IExampleService.cs#L17 "ValueTask<IEnumerable<Person>> IExampleService.ReadPeopleAsync(Expression<Func<Person, bool>> matches)") | 100 | 1 :heavy_check_mark: | 0 | 5 | 1 / 0 |
| Method | [16](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/IExampleService.cs#L16 "ValueTask<Person> IExampleService.ReadPersonByIdAsync(string id, string partitionKey)") | 100 | 1 :heavy_check_mark: | 0 | 2 | 1 / 0 |
| Method | [19](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/IExampleService.cs#L19 "ValueTask<Person> IExampleService.UpdatePersonAsync(Person person)") | 100 | 1 :heavy_check_mark: | 0 | 2 | 1 / 0 |

<a href="#servicetier">:top: back to ServiceTier</a>

</details>

<details>
<summary>
  <strong id="person">
    Person :heavy_check_mark:
  </strong>
</summary>
<br>

- The `Person` contains 8 members.
- 24 total lines of source code.
- Approximately 12 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Property | [21](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/Person.cs#L21 "int Person.AgeInYears") | 94 | 2 :heavy_check_mark: | 0 | 3 | 3 / 2 |
| Property | [14](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/Person.cs#L14 "DateTime Person.BirthDate") | 100 | 2 :heavy_check_mark: | 0 | 1 | 1 / 0 |
| Property | [16](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/Person.cs#L16 "string Person.FirstName") | 100 | 2 :heavy_check_mark: | 0 | 0 | 1 / 1 |
| Method | [28](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/Person.cs#L28 "string Person.GetPartitionKeyValue()") | 100 | 1 :heavy_check_mark: | 0 | 0 | 1 / 1 |
| Property | [18](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/Person.cs#L18 "string Person.LastName") | 100 | 2 :heavy_check_mark: | 0 | 0 | 1 / 1 |
| Property | [17](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/Person.cs#L17 "string? Person.MiddleName") | 100 | 2 :heavy_check_mark: | 0 | 1 | 1 / 0 |
| Property | [25](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/Person.cs#L25 "string Person.SyntheticPartitionKey") | 95 | 2 :heavy_check_mark: | 0 | 1 | 3 / 4 |
| Method | [30](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/Person.cs#L30 "string Person.ToString()") | 87 | 2 :heavy_check_mark: | 0 | 2 | 4 / 1 |

<a href="#servicetier">:top: back to ServiceTier</a>

</details>

<details>
<summary>
  <strong id="program">
    Program :heavy_check_mark:
  </strong>
</summary>
<br>

- The `Program` contains 5 members.
- 211 total lines of source code.
- Approximately 85 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [37](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/Program.cs#L37 "IHostBuilder Program.CreateHostBuilder(string[] args)") | 67 | 1 :heavy_check_mark: | 0 | 2 | 17 / 9 |
| Method | [19](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/Program.cs#L19 "Task Program.Main(string[] args)") | 74 | 1 :heavy_check_mark: | 0 | 5 | 17 / 5 |
| Method | [55](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/Program.cs#L55 "Task Program.RawRepositoryExampleAsync(IRepository<Person> repository)") | 52 | 2 :heavy_check_mark: | 0 | 6 | 49 / 20 |
| Method | [105](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/Program.cs#L105 "Task Program.RawRepositoryExampleAsync(IRepository<Widget> repository)") | 48 | 2 :heavy_check_mark: | 0 | 8 | 72 / 31 |
| Method | [178](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/Program.cs#L178 "Task Program.ServiceExampleAsync(IExampleService service)") | 52 | 2 :heavy_check_mark: | 0 | 8 | 49 / 20 |

<a href="#servicetier">:top: back to ServiceTier</a>

</details>

<details>
<summary>
  <strong id="widget">
    Widget :heavy_check_mark:
  </strong>
</summary>
<br>

- The `Widget` contains 3 members.
- 8 total lines of source code.
- Approximately 3 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Property | [13](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/Widget.cs#L13 "DateTimeOffset Widget.CreatedOrUpdatedOn") | 100 | 2 :heavy_check_mark: | 0 | 2 | 1 / 1 |
| Property | [11](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/Widget.cs#L11 "string Widget.Name") | 100 | 2 :heavy_check_mark: | 0 | 0 | 1 / 1 |
| Method | [15](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/Widget.cs#L15 "string Widget.ToString()") | 95 | 1 :heavy_check_mark: | 0 | 1 | 1 / 1 |

<a href="#servicetier">:top: back to ServiceTier</a>

</details>

</details>

<a href="#servicetier">:top: back to ServiceTier</a>

<div id='webtier'></div>

## WebTier :heavy_check_mark:

The *WebTier.csproj* project file contains:

- 3 namespaces.
- 5 named types.
- 106 total lines of source code.
- Approximately 31 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

<details>
<summary>
  <strong id="webtier-controllers">
    WebTier.Controllers :heavy_check_mark:
  </strong>
</summary>
<br>

The `WebTier.Controllers` namespace contains 1 named types.

- 1 named types.
- 32 total lines of source code.
- Approximately 14 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

<details>
<summary>
  <strong id="languagecontroller">
    LanguageController :heavy_check_mark:
  </strong>
</summary>
<br>

- The `LanguageController` contains 7 members.
- 29 total lines of source code.
- Approximately 14 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [17](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/WebTier/Controllers/LanguageController.cs#L17 "IRepository<Language> LanguageController._repository") | 100 | 0 :heavy_check_mark: | 0 | 2 | 1 / 0 |
| Method | [19](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/WebTier/Controllers/LanguageController.cs#L19 "LanguageController.LanguageController(IRepositoryFactory factory)") | 96 | 1 :heavy_check_mark: | 0 | 4 | 2 / 1 |
| Method | [39](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/WebTier/Controllers/LanguageController.cs#L39 "ValueTask LanguageController.DeleteLanguage(string id)") | 86 | 1 :heavy_check_mark: | 0 | 5 | 3 / 2 |
| Method | [27](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/WebTier/Controllers/LanguageController.cs#L27 "ValueTask<Language> LanguageController.GetLanguageById(string id)") | 86 | 1 :heavy_check_mark: | 0 | 5 | 3 / 2 |
| Method | [23](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/WebTier/Controllers/LanguageController.cs#L23 "ValueTask<IEnumerable<Language>> LanguageController.GetLanguages()") | 80 | 1 :heavy_check_mark: | 0 | 7 | 3 / 3 |
| Method | [31](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/WebTier/Controllers/LanguageController.cs#L31 "ValueTask<IEnumerable<Language>> LanguageController.PostLanguages(params Language[] languages)") | 87 | 1 :heavy_check_mark: | 0 | 7 | 3 / 2 |
| Method | [35](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/WebTier/Controllers/LanguageController.cs#L35 "ValueTask<Language> LanguageController.PutLanguage(Language language)") | 87 | 1 :heavy_check_mark: | 0 | 6 | 3 / 2 |

<a href="#webtier-controllers">:top: back to WebTier.Controllers</a>

</details>

</details>

<details>
<summary>
  <strong id="webtier-models">
    WebTier.Models :heavy_check_mark:
  </strong>
</summary>
<br>

The `WebTier.Models` namespace contains 2 named types.

- 2 named types.
- 28 total lines of source code.
- Approximately 0 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

<details>
<summary>
  <strong id="language">
    Language :heavy_check_mark:
  </strong>
</summary>
<br>

- The `Language` contains 5 members.
- 12 total lines of source code.
- Approximately 0 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Property | [13](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/WebTier/Models/Language.cs#L13 "string[] Language.Aliases") | 100 | 2 :heavy_check_mark: | 0 | 0 | 1 / 0 |
| Property | [15](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/WebTier/Models/Language.cs#L15 "string Language.Description") | 100 | 2 :heavy_check_mark: | 0 | 0 | 1 / 0 |
| Property | [19](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/WebTier/Models/Language.cs#L19 "DateTime Language.InitialReleaseDate") | 100 | 2 :heavy_check_mark: | 0 | 1 | 1 / 0 |
| Property | [11](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/WebTier/Models/Language.cs#L11 "string Language.Name") | 100 | 2 :heavy_check_mark: | 0 | 0 | 1 / 0 |
| Property | [17](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/WebTier/Models/Language.cs#L17 "ProgrammingStyle Language.PrimaryStyle") | 100 | 2 :heavy_check_mark: | 0 | 1 | 1 / 0 |

<a href="#webtier-models">:top: back to WebTier.Models</a>

</details>

<details>
<summary>
  <strong id="programmingstyle">
    ProgrammingStyle :heavy_check_mark:
  </strong>
</summary>
<br>

- The `ProgrammingStyle` contains 5 members.
- 8 total lines of source code.
- Approximately 0 lines of executable code.
- The highest cyclomatic complexity is 0 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [8](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/WebTier/Models/ProgrammingStyle.cs#L8 "ProgrammingStyle.Functional") | 100 | 0 :heavy_check_mark: | 0 | 0 | 1 / 0 |
| Field | [12](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/WebTier/Models/ProgrammingStyle.cs#L12 "ProgrammingStyle.Modular") | 100 | 0 :heavy_check_mark: | 0 | 0 | 1 / 0 |
| Field | [9](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/WebTier/Models/ProgrammingStyle.cs#L9 "ProgrammingStyle.ObjectOriented") | 100 | 0 :heavy_check_mark: | 0 | 0 | 1 / 0 |
| Field | [10](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/WebTier/Models/ProgrammingStyle.cs#L10 "ProgrammingStyle.Procedural") | 100 | 0 :heavy_check_mark: | 0 | 0 | 1 / 0 |
| Field | [11](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/WebTier/Models/ProgrammingStyle.cs#L11 "ProgrammingStyle.Script") | 100 | 0 :heavy_check_mark: | 0 | 0 | 1 / 0 |

<a href="#webtier-models">:top: back to WebTier.Models</a>

</details>

</details>

<details>
<summary>
  <strong id="webtier">
    WebTier :heavy_check_mark:
  </strong>
</summary>
<br>

The `WebTier` namespace contains 2 named types.

- 2 named types.
- 46 total lines of source code.
- Approximately 17 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

<details>
<summary>
  <strong id="program">
    Program :heavy_check_mark:
  </strong>
</summary>
<br>

- The `Program` contains 2 members.
- 9 total lines of source code.
- Approximately 3 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [15](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/WebTier/Program.cs#L15 "IHostBuilder Program.CreateHostBuilder(string[] args)") | 91 | 1 :heavy_check_mark: | 0 | 2 | 3 / 2 |
| Method | [12](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/WebTier/Program.cs#L12 "Task Program.Main(string[] args)") | 97 | 1 :heavy_check_mark: | 0 | 3 | 2 / 1 |

<a href="#webtier">:top: back to WebTier</a>

</details>

<details>
<summary>
  <strong id="startup">
    Startup :heavy_check_mark:
  </strong>
</summary>
<br>

- The `Startup` contains 2 members.
- 31 total lines of source code.
- Approximately 14 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [30](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/WebTier/Startup.cs#L30 "void Startup.Configure(IApplicationBuilder app, IWebHostEnvironment env)") | 69 | 1 :heavy_check_mark: | 0 | 3 | 12 / 9 |
| Method | [14](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/WebTier/Startup.cs#L14 "void Startup.ConfigureServices(IServiceCollection services)") | 75 | 1 :heavy_check_mark: | 0 | 4 | 15 / 5 |

<a href="#webtier">:top: back to WebTier</a>

</details>

</details>

<a href="#webtier">:top: back to WebTier</a>

<div id='microsoft-azure-cosmosrepository'></div>

## Microsoft.Azure.CosmosRepository :radioactive:

The *Microsoft.Azure.CosmosRepository.csproj* project file contains:

- 6 namespaces.
- 19 named types.
- 966 total lines of source code.
- Approximately 193 lines of executable code.
- The highest cyclomatic complexity is 11 :radioactive:.

<details>
<summary>
  <strong id="microsoft-azure-cosmosrepository-attributes">
    Microsoft.Azure.CosmosRepository.Attributes :heavy_check_mark:
  </strong>
</summary>
<br>

The `Microsoft.Azure.CosmosRepository.Attributes` namespace contains 1 named types.

- 1 named types.
- 29 total lines of source code.
- Approximately 4 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

<details>
<summary>
  <strong id="partitionkeypathattribute">
    PartitionKeyPathAttribute :heavy_check_mark:
  </strong>
</summary>
<br>

- The `PartitionKeyPathAttribute` contains 2 members.
- 26 total lines of source code.
- Approximately 4 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [31](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Attributes/PartitionKeyPathAttribute.cs#L31 "PartitionKeyPathAttribute.PartitionKeyPathAttribute(string path)") | 91 | 2 :heavy_check_mark: | 0 | 3 | 6 / 1 |
| Property | [25](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Attributes/PartitionKeyPathAttribute.cs#L25 "string PartitionKeyPathAttribute.Path") | 100 | 1 :heavy_check_mark: | 0 | 0 | 4 / 1 |

<a href="#microsoft-azure-cosmosrepository-attributes">:top: back to Microsoft.Azure.CosmosRepository.Attributes</a>

</details>

</details>

<details>
<summary>
  <strong id="microsoft-azure-cosmosrepository">
    Microsoft.Azure.CosmosRepository :heavy_check_mark:
  </strong>
</summary>
<br>

The `Microsoft.Azure.CosmosRepository` namespace contains 6 named types.

- 6 named types.
- 475 total lines of source code.
- Approximately 115 lines of executable code.
- The highest cyclomatic complexity is 4 :heavy_check_mark:.

<details>
<summary>
  <strong id="defaultrepositorytitem">
    DefaultRepository&lt;TItem&gt; :heavy_check_mark:
  </strong>
</summary>
<br>

- The `DefaultRepository<TItem>` contains 18 members.
- 216 total lines of source code.
- Approximately 78 lines of executable code.
- The highest cyclomatic complexity is 4 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [24](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L24 "ICosmosContainerProvider<TItem> DefaultRepository<TItem>._containerProvider") | 100 | 0 :heavy_check_mark: | 0 | 1 | 1 / 0 |
| Field | [26](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L26 "ILogger<DefaultRepository<TItem>> DefaultRepository<TItem>._logger") | 100 | 0 :heavy_check_mark: | 0 | 1 | 1 / 0 |
| Field | [25](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L25 "IOptionsMonitor<RepositoryOptions> DefaultRepository<TItem>._optionsMonitor") | 100 | 0 :heavy_check_mark: | 0 | 2 | 1 / 0 |
| Method | [34](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L34 "DefaultRepository<TItem>.DefaultRepository(IOptionsMonitor<RepositoryOptions> optionsMonitor, ICosmosContainerProvider<TItem> containerProvider, ILogger<DefaultRepository<TItem>> logger)") | 89 | 1 :heavy_check_mark: | 0 | 5 | 5 / 1 |
| Method | [130](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L130 "ValueTask<TItem> DefaultRepository<TItem>.CreateAsync(TItem value, CancellationToken cancellationToken = null)") | 68 | 1 :heavy_check_mark: | 0 | 9 | 16 / 6 |
| Method | [147](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L147 "ValueTask<IEnumerable<TItem>> DefaultRepository<TItem>.CreateAsync(IEnumerable<TItem> values, CancellationToken cancellationToken = null)") | 71 | 1 :heavy_check_mark: | 0 | 5 | 13 / 6 |
| Method | [178](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L178 "ValueTask DefaultRepository<TItem>.DeleteAsync(TItem value, CancellationToken cancellationToken = null)") | 80 | 1 :heavy_check_mark: | 0 | 3 | 5 / 2 |
| Method | [184](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L184 "ValueTask DefaultRepository<TItem>.DeleteAsync(string id, string partitionKeyValue = null, CancellationToken cancellationToken = null)") | 76 | 2 :heavy_check_mark: | 0 | 4 | 6 / 3 |
| Method | [191](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L191 "ValueTask DefaultRepository<TItem>.DeleteAsync(string id, PartitionKey partitionKey, CancellationToken cancellationToken = null)") | 65 | 2 :heavy_check_mark: | 0 | 10 | 19 / 8 |
| Method | [41](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L41 "ValueTask<TItem> DefaultRepository<TItem>.GetAsync(string id, string partitionKeyValue = null, CancellationToken cancellationToken = null)") | 76 | 2 :heavy_check_mark: | 0 | 4 | 6 / 3 |
| Method | [48](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L48 "ValueTask<TItem> DefaultRepository<TItem>.GetAsync(string id, PartitionKey partitionKey, CancellationToken cancellationToken = null)") | 62 | 4 :heavy_check_mark: | 0 | 10 | 24 / 9 |
| Method | [73](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L73 "ValueTask<IEnumerable<TItem>> DefaultRepository<TItem>.GetAsync(Expression<Func<TItem, bool>> predicate, CancellationToken cancellationToken = null)") | 60 | 4 :heavy_check_mark: | 0 | 14 | 29 / 12 |
| Method | [103](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L103 "ValueTask<IEnumerable<TItem>> DefaultRepository<TItem>.GetByQueryAsync(string query, CancellationToken cancellationToken = null)") | 69 | 1 :heavy_check_mark: | 0 | 9 | 13 / 6 |
| Method | [117](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L117 "ValueTask<IEnumerable<TItem>> DefaultRepository<TItem>.GetByQueryAsync(QueryDefinition queryDefinition, CancellationToken cancellationToken = null)") | 71 | 1 :heavy_check_mark: | 0 | 9 | 12 / 5 |
| Method | [210](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L210 "Task<IEnumerable<TItem>> DefaultRepository<TItem>.IterateQueryInternalAsync(Container container, QueryDefinition queryDefinition, CancellationToken cancellationToken)") | 71 | 2 :heavy_check_mark: | 0 | 9 | 17 / 6 |
| Property | [28](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L28 "(bool OptimizeBandwidth, ItemRequestOptions Options) DefaultRepository<TItem>.RequestOptions") | 100 | 2 :heavy_check_mark: | 0 | 5 | 5 / 2 |
| Method | [229](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L229 "void DefaultRepository<TItem>.TryLogDebugDetails(ILogger logger, Func<string> getMessage)") | 86 | 4 :heavy_check_mark: | 0 | 3 | 7 / 2 |
| Method | [161](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L161 "ValueTask<TItem> DefaultRepository<TItem>.UpdateAsync(TItem value, CancellationToken cancellationToken = null)") | 66 | 2 :heavy_check_mark: | 0 | 11 | 16 / 7 |

<a href="#microsoft-azure-cosmosrepository">:top: back to Microsoft.Azure.CosmosRepository</a>

</details>

<details>
<summary>
  <strong id="defaultrepositoryfactory">
    DefaultRepositoryFactory :heavy_check_mark:
  </strong>
</summary>
<br>

- The `DefaultRepositoryFactory` contains 3 members.
- 20 total lines of source code.
- Approximately 2 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [12](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepositoryFactory.cs#L12 "IServiceProvider DefaultRepositoryFactory._serviceProvider") | 100 | 0 :heavy_check_mark: | 0 | 1 | 1 / 0 |
| Method | [18](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepositoryFactory.cs#L18 "DefaultRepositoryFactory.DefaultRepositoryFactory(IServiceProvider serviceProvider)") | 91 | 2 :heavy_check_mark: | 0 | 4 | 9 / 1 |
| Method | [25](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepositoryFactory.cs#L25 "IRepository<TItem> DefaultRepositoryFactory.RepositoryOf<TItem>()") | 100 | 1 :heavy_check_mark: | 0 | 3 | 4 / 1 |

<a href="#microsoft-azure-cosmosrepository">:top: back to Microsoft.Azure.CosmosRepository</a>

</details>

<details>
<summary>
  <strong id="iitem">
    IItem :heavy_check_mark:
  </strong>
</summary>
<br>

- The `IItem` contains 3 members.
- 20 total lines of source code.
- Approximately 0 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Property | [16](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/IItem.cs#L16 "string IItem.Id") | 100 | 2 :heavy_check_mark: | 0 | 0 | 4 / 0 |
| Property | [26](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/IItem.cs#L26 "PartitionKey IItem.PartitionKey") | 100 | 1 :heavy_check_mark: | 0 | 1 | 4 / 0 |
| Property | [21](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/IItem.cs#L21 "string IItem.Type") | 100 | 2 :heavy_check_mark: | 0 | 0 | 4 / 0 |

<a href="#microsoft-azure-cosmosrepository">:top: back to Microsoft.Azure.CosmosRepository</a>

</details>

<details>
<summary>
  <strong id="irepositorytitem">
    IRepository&lt;TItem&gt; :heavy_check_mark:
  </strong>
</summary>
<br>

- The `IRepository<TItem>` contains 11 members.
- 125 total lines of source code.
- Approximately 26 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [94](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/IRepository.cs#L94 "ValueTask<TItem> IRepository<TItem>.CreateAsync(TItem value, CancellationToken cancellationToken = null)") | 87 | 1 :heavy_check_mark: | 0 | 2 | 7 / 2 |
| Method | [102](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/IRepository.cs#L102 "ValueTask<IEnumerable<TItem>> IRepository<TItem>.CreateAsync(IEnumerable<TItem> values, CancellationToken cancellationToken = null)") | 87 | 1 :heavy_check_mark: | 0 | 3 | 7 / 2 |
| Method | [118](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/IRepository.cs#L118 "ValueTask IRepository<TItem>.DeleteAsync(TItem value, CancellationToken cancellationToken = null)") | 87 | 1 :heavy_check_mark: | 0 | 2 | 7 / 2 |
| Method | [127](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/IRepository.cs#L127 "ValueTask IRepository<TItem>.DeleteAsync(string id, string partitionKeyValue = null, CancellationToken cancellationToken = null)") | 80 | 1 :heavy_check_mark: | 0 | 2 | 8 / 4 |
| Method | [136](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/IRepository.cs#L136 "ValueTask IRepository<TItem>.DeleteAsync(string id, PartitionKey partitionKey, CancellationToken cancellationToken = null)") | 87 | 1 :heavy_check_mark: | 0 | 3 | 8 / 2 |
| Method | [44](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/IRepository.cs#L44 "ValueTask<TItem> IRepository<TItem>.GetAsync(string id, string partitionKeyValue = null, CancellationToken cancellationToken = null)") | 80 | 1 :heavy_check_mark: | 0 | 2 | 11 / 4 |
| Method | [56](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/IRepository.cs#L56 "ValueTask<TItem> IRepository<TItem>.GetAsync(string id, PartitionKey partitionKey, CancellationToken cancellationToken = null)") | 87 | 1 :heavy_check_mark: | 0 | 3 | 11 / 2 |
| Method | [68](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/IRepository.cs#L68 "ValueTask<IEnumerable<TItem>> IRepository<TItem>.GetAsync(Expression<Func<TItem, bool>> predicate, CancellationToken cancellationToken = null)") | 87 | 1 :heavy_check_mark: | 0 | 5 | 11 / 2 |
| Method | [77](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/IRepository.cs#L77 "ValueTask<IEnumerable<TItem>> IRepository<TItem>.GetByQueryAsync(string query, CancellationToken cancellationToken = null)") | 87 | 1 :heavy_check_mark: | 0 | 3 | 8 / 2 |
| Method | [86](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/IRepository.cs#L86 "ValueTask<IEnumerable<TItem>> IRepository<TItem>.GetByQueryAsync(QueryDefinition queryDefinition, CancellationToken cancellationToken = null)") | 87 | 1 :heavy_check_mark: | 0 | 4 | 8 / 2 |
| Method | [110](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/IRepository.cs#L110 "ValueTask<TItem> IRepository<TItem>.UpdateAsync(TItem value, CancellationToken cancellationToken = null)") | 87 | 1 :heavy_check_mark: | 0 | 2 | 7 / 2 |

<a href="#microsoft-azure-cosmosrepository">:top: back to Microsoft.Azure.CosmosRepository</a>

</details>

<details>
<summary>
  <strong id="irepositoryfactory">
    IRepositoryFactory :heavy_check_mark:
  </strong>
</summary>
<br>

- The `IRepositoryFactory` contains 1 members.
- 14 total lines of source code.
- Approximately 0 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [18](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/IRepositoryFactory.cs#L18 "IRepository<TItem> IRepositoryFactory.RepositoryOf<TItem>()") | 100 | 1 :heavy_check_mark: | 0 | 2 | 7 / 0 |

<a href="#microsoft-azure-cosmosrepository">:top: back to Microsoft.Azure.CosmosRepository</a>

</details>

<details>
<summary>
  <strong id="item">
    Item :heavy_check_mark:
  </strong>
</summary>
<br>

- The `Item` contains 5 members.
- 60 total lines of source code.
- Approximately 9 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [60](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Item.cs#L60 "Item.Item()") | 100 | 1 :heavy_check_mark: | 0 | 1 | 4 / 1 |
| Method | [69](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Item.cs#L69 "string Item.GetPartitionKeyValue()") | 100 | 1 :heavy_check_mark: | 0 | 0 | 8 / 1 |
| Property | [43](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Item.cs#L43 "string Item.Id") | 100 | 2 :heavy_check_mark: | 0 | 2 | 8 / 3 |
| Property | [55](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Item.cs#L55 "PartitionKey Item.PartitionKey") | 100 | 2 :heavy_check_mark: | 0 | 2 | 5 / 2 |
| Property | [49](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Item.cs#L49 "string Item.Type") | 100 | 2 :heavy_check_mark: | 0 | 1 | 5 / 2 |

<a href="#microsoft-azure-cosmosrepository">:top: back to Microsoft.Azure.CosmosRepository</a>

</details>

</details>

<details>
<summary>
  <strong id="microsoft-extensions-dependencyinjection">
    Microsoft.Extensions.DependencyInjection :heavy_check_mark:
  </strong>
</summary>
<br>

The `Microsoft.Extensions.DependencyInjection` namespace contains 1 named types.

- 1 named types.
- 64 total lines of source code.
- Approximately 12 lines of executable code.
- The highest cyclomatic complexity is 3 :heavy_check_mark:.

<details>
<summary>
  <strong id="servicecollectionextensions">
    ServiceCollectionExtensions :heavy_check_mark:
  </strong>
</summary>
<br>

- The `ServiceCollectionExtensions` contains 2 members.
- 61 total lines of source code.
- Approximately 12 lines of executable code.
- The highest cyclomatic complexity is 3 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [24](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Extensions/ServiceCollectionExtensions.cs#L24 "IServiceCollection ServiceCollectionExtensions.AddCosmosRepository(IServiceCollection services, Action<RepositoryOptions> setupAction = null)") | 65 | 3 :heavy_check_mark: | 0 | 7 | 38 / 9 |
| Method | [67](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Extensions/ServiceCollectionExtensions.cs#L67 "IServiceCollection ServiceCollectionExtensions.AddCosmosRepository(IServiceCollection services, IConfiguration configuration, Action<RepositoryOptions> setupAction = null)") | 78 | 1 :heavy_check_mark: | 0 | 6 | 16 / 3 |

<a href="#microsoft-extensions-dependencyinjection">:top: back to Microsoft.Extensions.DependencyInjection</a>

</details>

</details>

<details>
<summary>
  <strong id="microsoft-azure-cosmosrepository-extensions">
    Microsoft.Azure.CosmosRepository.Extensions :heavy_check_mark:
  </strong>
</summary>
<br>

The `Microsoft.Azure.CosmosRepository.Extensions` namespace contains 2 named types.

- 2 named types.
- 63 total lines of source code.
- Approximately 14 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

<details>
<summary>
  <strong id="expressionextensions">
    ExpressionExtensions :heavy_check_mark:
  </strong>
</summary>
<br>

- The `ExpressionExtensions` contains 4 members.
- 34 total lines of source code.
- Approximately 9 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [37](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Extensions/ExpressionExtensions.cs#L37 "Expression<Func<T, bool>> ExpressionExtensions.And<T>(Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)") | 98 | 1 :heavy_check_mark: | 0 | 3 | 3 / 1 |
| Method | [33](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Extensions/ExpressionExtensions.cs#L33 "Expression<Func<T, bool>> ExpressionExtensions.AndAlso<T>(Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)") | 98 | 1 :heavy_check_mark: | 0 | 3 | 3 / 1 |
| Method | [17](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Extensions/ExpressionExtensions.cs#L17 "Expression<T> ExpressionExtensions.Compose<T>(Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)") | 71 | 1 :heavy_check_mark: | 0 | 7 | 15 / 6 |
| Method | [41](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Extensions/ExpressionExtensions.cs#L41 "Expression<Func<T, bool>> ExpressionExtensions.Or<T>(Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)") | 98 | 1 :heavy_check_mark: | 0 | 3 | 3 / 1 |

<a href="#microsoft-azure-cosmosrepository-extensions">:top: back to Microsoft.Azure.CosmosRepository.Extensions</a>

</details>

<details>
<summary>
  <strong id="parameterrebinder">
    ParameterRebinder :heavy_check_mark:
  </strong>
</summary>
<br>

- The `ParameterRebinder` contains 4 members.
- 23 total lines of source code.
- Approximately 5 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [11](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Extensions/ParameterRebinder.cs#L11 "IDictionary<ParameterExpression, ParameterExpression> ParameterRebinder._map") | 100 | 0 :heavy_check_mark: | 0 | 2 | 1 / 0 |
| Method | [13](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Extensions/ParameterRebinder.cs#L13 "ParameterRebinder.ParameterRebinder(IDictionary<ParameterExpression, ParameterExpression> map)") | 95 | 2 :heavy_check_mark: | 0 | 4 | 2 / 1 |
| Method | [16](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Extensions/ParameterRebinder.cs#L16 "Expression ParameterRebinder.ReplaceParameters(IDictionary<ParameterExpression, ParameterExpression> map, Expression exp)") | 94 | 1 :heavy_check_mark: | 0 | 4 | 3 / 1 |
| Method | [21](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Extensions/ParameterRebinder.cs#L21 "Expression ParameterRebinder.VisitParameter(ParameterExpression parameter)") | 81 | 2 :heavy_check_mark: | 0 | 5 | 10 / 3 |

<a href="#microsoft-azure-cosmosrepository-extensions">:top: back to Microsoft.Azure.CosmosRepository.Extensions</a>

</details>

</details>

<details>
<summary>
  <strong id="microsoft-azure-cosmosrepository-options">
    Microsoft.Azure.CosmosRepository.Options :heavy_check_mark:
  </strong>
</summary>
<br>

The `Microsoft.Azure.CosmosRepository.Options` namespace contains 1 named types.

- 1 named types.
- 63 total lines of source code.
- Approximately 3 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

<details>
<summary>
  <strong id="repositoryoptions">
    RepositoryOptions :heavy_check_mark:
  </strong>
</summary>
<br>

- The `RepositoryOptions` contains 6 members.
- 60 total lines of source code.
- Approximately 3 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Property | [66](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Options/RepositoryOptions.cs#L66 "bool RepositoryOptions.AllowBulkExecution") | 100 | 2 :heavy_check_mark: | 0 | 0 | 9 / 0 |
| Property | [33](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Options/RepositoryOptions.cs#L33 "string RepositoryOptions.ContainerId") | 100 | 2 :heavy_check_mark: | 0 | 0 | 7 / 1 |
| Property | [55](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Options/RepositoryOptions.cs#L55 "bool RepositoryOptions.ContainerPerItemType") | 100 | 2 :heavy_check_mark: | 0 | 0 | 9 / 0 |
| Property | [17](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Options/RepositoryOptions.cs#L17 "string RepositoryOptions.CosmosConnectionString") | 100 | 2 :heavy_check_mark: | 0 | 0 | 4 / 0 |
| Property | [25](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Options/RepositoryOptions.cs#L25 "string RepositoryOptions.DatabaseId") | 100 | 2 :heavy_check_mark: | 0 | 0 | 7 / 1 |
| Property | [45](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Options/RepositoryOptions.cs#L45 "bool RepositoryOptions.OptimizeBandwidth") | 100 | 2 :heavy_check_mark: | 0 | 0 | 11 / 1 |

<a href="#microsoft-azure-cosmosrepository-options">:top: back to Microsoft.Azure.CosmosRepository.Options</a>

</details>

</details>

<details>
<summary>
  <strong id="microsoft-azure-cosmosrepository-providers">
    Microsoft.Azure.CosmosRepository.Providers :radioactive:
  </strong>
</summary>
<br>

The `Microsoft.Azure.CosmosRepository.Providers` namespace contains 8 named types.

- 8 named types.
- 272 total lines of source code.
- Approximately 45 lines of executable code.
- The highest cyclomatic complexity is 11 :radioactive:.

<details>
<summary>
  <strong id="defaultcosmosclientoptionsprovider">
    DefaultCosmosClientOptionsProvider :heavy_check_mark:
  </strong>
</summary>
<br>

- The `DefaultCosmosClientOptionsProvider` contains 5 members.
- 47 total lines of source code.
- Approximately 10 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [14](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosClientOptionsProvider.cs#L14 "Lazy<CosmosClientOptions> DefaultCosmosClientOptionsProvider._lazyClientOptions") | 100 | 0 :heavy_check_mark: | 0 | 2 | 1 / 0 |
| Method | [24](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosClientOptionsProvider.cs#L24 "DefaultCosmosClientOptionsProvider.DefaultCosmosClientOptionsProvider(IServiceProvider serviceProvider, IConfiguration configuration)") | 85 | 1 :heavy_check_mark: | 0 | 4 | 10 / 2 |
| Method | [41](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosClientOptionsProvider.cs#L41 "HttpClient DefaultCosmosClientOptionsProvider.ClientFactory(IServiceProvider serviceProvider)") | 78 | 1 :heavy_check_mark: | 0 | 3 | 16 / 4 |
| Property | [17](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosClientOptionsProvider.cs#L17 "CosmosClientOptions DefaultCosmosClientOptionsProvider.ClientOptions") | 100 | 2 :heavy_check_mark: | 0 | 3 | 2 / 2 |
| Method | [30](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosClientOptionsProvider.cs#L30 "CosmosClientOptions DefaultCosmosClientOptionsProvider.CreateCosmosClientOptions(IServiceProvider serviceProvider, IConfiguration configuration)") | 83 | 1 :heavy_check_mark: | 0 | 5 | 10 / 2 |

<a href="#microsoft-azure-cosmosrepository-providers">:top: back to Microsoft.Azure.CosmosRepository.Providers</a>

</details>

<details>
<summary>
  <strong id="defaultcosmosclientprovider">
    DefaultCosmosClientProvider :heavy_check_mark:
  </strong>
</summary>
<br>

- The `DefaultCosmosClientProvider` contains 7 members.
- 46 total lines of source code.
- Approximately 8 lines of executable code.
- The highest cyclomatic complexity is 6 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [16](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosClientProvider.cs#L16 "CosmosClientOptions DefaultCosmosClientProvider._cosmosClientOptions") | 100 | 0 :heavy_check_mark: | 0 | 1 | 1 / 0 |
| Field | [15](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosClientProvider.cs#L15 "Lazy<CosmosClient> DefaultCosmosClientProvider._lazyCosmosClient") | 100 | 0 :heavy_check_mark: | 0 | 2 | 1 / 0 |
| Field | [17](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosClientProvider.cs#L17 "RepositoryOptions DefaultCosmosClientProvider._options") | 100 | 0 :heavy_check_mark: | 0 | 1 | 1 / 0 |
| Method | [20](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosClientProvider.cs#L20 "DefaultCosmosClientProvider.DefaultCosmosClientProvider(CosmosClientOptions cosmosClientOptions, IOptions<RepositoryOptions> options)") | 73 | 4 :heavy_check_mark: | 0 | 8 | 16 / 4 |
| Method | [37](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosClientProvider.cs#L37 "DefaultCosmosClientProvider.DefaultCosmosClientProvider(ICosmosClientOptionsProvider cosmosClientOptionsProvider, IOptions<RepositoryOptions> options)") | 89 | 3 :heavy_check_mark: | 0 | 7 | 8 / 1 |
| Method | [50](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosClientProvider.cs#L50 "void DefaultCosmosClientProvider.Dispose()") | 86 | 6 :heavy_check_mark: | 0 | 3 | 8 / 2 |
| Method | [46](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosClientProvider.cs#L46 "Task<T> DefaultCosmosClientProvider.UseClientAsync<T>(Func<CosmosClient, Task<T>> consume)") | 96 | 2 :heavy_check_mark: | 0 | 5 | 3 / 1 |

<a href="#microsoft-azure-cosmosrepository-providers">:top: back to Microsoft.Azure.CosmosRepository.Providers</a>

</details>

<details>
<summary>
  <strong id="defaultcosmoscontainerprovidertitem">
    DefaultCosmosContainerProvider&lt;TItem&gt; :radioactive:
  </strong>
</summary>
<br>

- The `DefaultCosmosContainerProvider<TItem>` contains 7 members.
- 83 total lines of source code.
- Approximately 22 lines of executable code.
- The highest cyclomatic complexity is 11 :radioactive:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [19](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosContainerProvider.cs#L19 "ICosmosClientProvider DefaultCosmosContainerProvider<TItem>._cosmosClientProvider") | 100 | 0 :heavy_check_mark: | 0 | 1 | 1 / 0 |
| Field | [20](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosContainerProvider.cs#L20 "ICosmosPartitionKeyPathProvider DefaultCosmosContainerProvider<TItem>._cosmosPartitionKeyPathProvider") | 100 | 0 :heavy_check_mark: | 0 | 1 | 1 / 0 |
| Field | [17](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosContainerProvider.cs#L17 "Lazy<Task<Container>> DefaultCosmosContainerProvider<TItem>._lazyContainer") | 100 | 0 :heavy_check_mark: | 0 | 3 | 1 / 0 |
| Field | [21](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosContainerProvider.cs#L21 "ILogger<DefaultCosmosContainerProvider<TItem>> DefaultCosmosContainerProvider<TItem>._logger") | 100 | 0 :heavy_check_mark: | 0 | 1 | 1 / 0 |
| Field | [18](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosContainerProvider.cs#L18 "RepositoryOptions DefaultCosmosContainerProvider<TItem>._options") | 100 | 0 :heavy_check_mark: | 0 | 1 | 1 / 0 |
| Method | [23](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosContainerProvider.cs#L23 "DefaultCosmosContainerProvider<TItem>.DefaultCosmosContainerProvider(ICosmosClientProvider cosmosClientProvider, ICosmosPartitionKeyPathProvider cosmosPartitionKeyPathProvider, IOptions<RepositoryOptions> options, ILogger<DefaultCosmosContainerProvider<TItem>> logger)") | 50 | 11 :radioactive: | 0 | 15 | 69 / 21 |
| Method | [94](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosContainerProvider.cs#L94 "Task<Container> DefaultCosmosContainerProvider<TItem>.GetContainerAsync()") | 100 | 1 :heavy_check_mark: | 0 | 4 | 2 / 1 |

<a href="#microsoft-azure-cosmosrepository-providers">:top: back to Microsoft.Azure.CosmosRepository.Providers</a>

</details>

<details>
<summary>
  <strong id="defaultcosmospartitionkeypathprovider">
    DefaultCosmosPartitionKeyPathProvider :heavy_check_mark:
  </strong>
</summary>
<br>

- The `DefaultCosmosPartitionKeyPathProvider` contains 4 members.
- 21 total lines of source code.
- Approximately 5 lines of executable code.
- The highest cyclomatic complexity is 3 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [15](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosPartitionKeyPathProvider.cs#L15 "ConcurrentDictionary<Type, string> DefaultCosmosPartitionKeyPathProvider._partionKeyNameMap") | 93 | 0 :heavy_check_mark: | 0 | 2 | 1 / 1 |
| Field | [14](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosPartitionKeyPathProvider.cs#L14 "Type DefaultCosmosPartitionKeyPathProvider._partitionKeyNameAttributeType") | 93 | 0 :heavy_check_mark: | 0 | 2 | 1 / 1 |
| Method | [22](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosPartitionKeyPathProvider.cs#L22 "string DefaultCosmosPartitionKeyPathProvider.GetPartitionKeyNameFactory(Type type)") | 83 | 3 :heavy_check_mark: | 0 | 3 | 8 / 2 |
| Method | [19](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosPartitionKeyPathProvider.cs#L19 "string DefaultCosmosPartitionKeyPathProvider.GetPartitionKeyPath<TItem>()") | 100 | 1 :heavy_check_mark: | 0 | 4 | 3 / 1 |

<a href="#microsoft-azure-cosmosrepository-providers">:top: back to Microsoft.Azure.CosmosRepository.Providers</a>

</details>

<details>
<summary>
  <strong id="icosmosclientoptionsprovider">
    ICosmosClientOptionsProvider :heavy_check_mark:
  </strong>
</summary>
<br>

- The `ICosmosClientOptionsProvider` contains 1 members.
- 13 total lines of source code.
- Approximately 0 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Property | [16](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/ICosmosClientOptionsProvider.cs#L16 "CosmosClientOptions ICosmosClientOptionsProvider.ClientOptions") | 100 | 1 :heavy_check_mark: | 0 | 1 | 5 / 0 |

<a href="#microsoft-azure-cosmosrepository-providers">:top: back to Microsoft.Azure.CosmosRepository.Providers</a>

</details>

<details>
<summary>
  <strong id="icosmosclientprovider">
    ICosmosClientProvider :heavy_check_mark:
  </strong>
</summary>
<br>

- The `ICosmosClientProvider` contains 1 members.
- 9 total lines of source code.
- Approximately 0 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [19](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/ICosmosClientProvider.cs#L19 "Task<T> ICosmosClientProvider.UseClientAsync<T>(Func<CosmosClient, Task<T>> consume)") | 100 | 1 :heavy_check_mark: | 0 | 3 | 1 / 0 |

<a href="#microsoft-azure-cosmosrepository-providers">:top: back to Microsoft.Azure.CosmosRepository.Providers</a>

</details>

<details>
<summary>
  <strong id="icosmoscontainerprovidertitem">
    ICosmosContainerProvider&lt;TItem&gt; :heavy_check_mark:
  </strong>
</summary>
<br>

- The `ICosmosContainerProvider<TItem>` contains 1 members.
- 13 total lines of source code.
- Approximately 0 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [21](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/ICosmosContainerProvider.cs#L21 "Task<Container> ICosmosContainerProvider<TItem>.GetContainerAsync()") | 100 | 1 :heavy_check_mark: | 0 | 2 | 6 / 0 |

<a href="#microsoft-azure-cosmosrepository-providers">:top: back to Microsoft.Azure.CosmosRepository.Providers</a>

</details>

<details>
<summary>
  <strong id="icosmospartitionkeypathprovider">
    ICosmosPartitionKeyPathProvider :heavy_check_mark:
  </strong>
</summary>
<br>

- The `ICosmosPartitionKeyPathProvider` contains 1 members.
- 13 total lines of source code.
- Approximately 0 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [17](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/ICosmosPartitionKeyPathProvider.cs#L17 "string ICosmosPartitionKeyPathProvider.GetPartitionKeyPath<TItem>()") | 100 | 1 :heavy_check_mark: | 0 | 1 | 6 / 0 |

<a href="#microsoft-azure-cosmosrepository-providers">:top: back to Microsoft.Azure.CosmosRepository.Providers</a>

</details>

</details>

<a href="#microsoft-azure-cosmosrepository">:top: back to Microsoft.Azure.CosmosRepository</a>

<div id='microsoft-azure-cosmosrepositorytests'></div>

## Microsoft.Azure.CosmosRepositoryTests :heavy_check_mark:

The *Microsoft.Azure.CosmosRepositoryTests.csproj* project file contains:

- 3 namespaces.
- 13 named types.
- 266 total lines of source code.
- Approximately 74 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

<details>
<summary>
  <strong id="microsoft-azure-cosmosrepositorytests">
    Microsoft.Azure.CosmosRepositoryTests :heavy_check_mark:
  </strong>
</summary>
<br>

The `Microsoft.Azure.CosmosRepositoryTests` namespace contains 5 named types.

- 5 named types.
- 64 total lines of source code.
- Approximately 26 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

<details>
<summary>
  <strong id="andacustomentity">
    AndACustomEntity :question:
  </strong>
</summary>
<br>

- The `AndACustomEntity` contains 0 members.
- 1 total lines of source code.
- Approximately 0 lines of executable code.
- The highest cyclomatic complexity is 0 :question:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |

<a href="#microsoft-azure-cosmosrepositorytests">:top: back to Microsoft.Azure.CosmosRepositoryTests</a>

</details>

<details>
<summary>
  <strong id="andanotheritem">
    AndAnotherItem :question:
  </strong>
</summary>
<br>

- The `AndAnotherItem` contains 0 members.
- 1 total lines of source code.
- Approximately 0 lines of executable code.
- The highest cyclomatic complexity is 0 :question:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |

<a href="#microsoft-azure-cosmosrepositorytests">:top: back to Microsoft.Azure.CosmosRepositoryTests</a>

</details>

<details>
<summary>
  <strong id="anothertestitem">
    AnotherTestItem :question:
  </strong>
</summary>
<br>

- The `AnotherTestItem` contains 0 members.
- 1 total lines of source code.
- Approximately 0 lines of executable code.
- The highest cyclomatic complexity is 0 :question:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |

<a href="#microsoft-azure-cosmosrepositorytests">:top: back to Microsoft.Azure.CosmosRepositoryTests</a>

</details>

<details>
<summary>
  <strong id="customentitybase">
    CustomEntityBase :heavy_check_mark:
  </strong>
</summary>
<br>

- The `CustomEntityBase` contains 8 members.
- 26 total lines of source code.
- Approximately 15 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [69](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/DefaultRepositoryFactoryTests.cs#L69 "CustomEntityBase.CustomEntityBase()") | 100 | 1 :heavy_check_mark: | 0 | 1 | 1 / 1 |
| Property | [65](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/DefaultRepositoryFactoryTests.cs#L65 "string CustomEntityBase.FavoriteColor") | 100 | 2 :heavy_check_mark: | 0 | 1 | 2 / 2 |
| Method | [71](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/DefaultRepositoryFactoryTests.cs#L71 "string CustomEntityBase.GetPartitionKeyValue()") | 100 | 1 :heavy_check_mark: | 0 | 0 | 1 / 1 |
| Property | [53](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/DefaultRepositoryFactoryTests.cs#L53 "string CustomEntityBase.Id") | 100 | 2 :heavy_check_mark: | 0 | 2 | 2 / 3 |
| Property | [67](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/DefaultRepositoryFactoryTests.cs#L67 "PartitionKey CustomEntityBase.PartitionKey") | 100 | 2 :heavy_check_mark: | 0 | 2 | 1 / 2 |
| Property | [59](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/DefaultRepositoryFactoryTests.cs#L59 "string CustomEntityBase.Name") | 100 | 2 :heavy_check_mark: | 0 | 1 | 2 / 2 |
| Property | [62](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/DefaultRepositoryFactoryTests.cs#L62 "string CustomEntityBase.Quest") | 100 | 2 :heavy_check_mark: | 0 | 1 | 2 / 2 |
| Property | [56](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/DefaultRepositoryFactoryTests.cs#L56 "string CustomEntityBase.Type") | 100 | 2 :heavy_check_mark: | 0 | 1 | 2 / 2 |

<a href="#microsoft-azure-cosmosrepositorytests">:top: back to Microsoft.Azure.CosmosRepositoryTests</a>

</details>

<details>
<summary>
  <strong id="defaultrepositoryfactorytests">
    DefaultRepositoryFactoryTests :heavy_check_mark:
  </strong>
</summary>
<br>

- The `DefaultRepositoryFactoryTests` contains 2 members.
- 30 total lines of source code.
- Approximately 11 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [15](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/DefaultRepositoryFactoryTests.cs#L15 "void DefaultRepositoryFactoryTests.NewRepositoryFactoryThrowsWithNullServiceProvider()") | 93 | 1 :heavy_check_mark: | 0 | 3 | 4 / 2 |
| Method | [20](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/DefaultRepositoryFactoryTests.cs#L20 "void DefaultRepositoryFactoryTests.RepositoryFactoryCorrectlyGetsRepositoryTest()") | 67 | 1 :heavy_check_mark: | 0 | 9 | 22 / 9 |

<a href="#microsoft-azure-cosmosrepositorytests">:top: back to Microsoft.Azure.CosmosRepositoryTests</a>

</details>

</details>

<details>
<summary>
  <strong id="microsoft-azure-cosmosrepositorytests-extensions">
    Microsoft.Azure.CosmosRepositoryTests.Extensions :heavy_check_mark:
  </strong>
</summary>
<br>

The `Microsoft.Azure.CosmosRepositoryTests.Extensions` namespace contains 2 named types.

- 2 named types.
- 35 total lines of source code.
- Approximately 10 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

<details>
<summary>
  <strong id="expressionextensiontests">
    ExpressionExtensionTests :heavy_check_mark:
  </strong>
</summary>
<br>

- The `ExpressionExtensionTests` contains 2 members.
- 22 total lines of source code.
- Approximately 8 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [25](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Extensions/ExpressionExtensionTests.cs#L25 "void ExpressionExtensionTests.ComposeCorrectlyAccountsForBothExpressions(DateTime arg, bool expected)") | 70 | 1 :heavy_check_mark: | 0 | 7 | 12 / 6 |
| Property | [14](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Extensions/ExpressionExtensionTests.cs#L14 "IEnumerable<object[]> ExpressionExtensionTests.CompositionInput") | 89 | 2 :heavy_check_mark: | 0 | 3 | 6 / 2 |

<a href="#microsoft-azure-cosmosrepositorytests-extensions">:top: back to Microsoft.Azure.CosmosRepositoryTests.Extensions</a>

</details>

<details>
<summary>
  <strong id="servicecollectionextensionstests">
    ServiceCollectionExtensionsTests :heavy_check_mark:
  </strong>
</summary>
<br>

- The `ServiceCollectionExtensionsTests` contains 1 members.
- 7 total lines of source code.
- Approximately 2 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [13](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Extensions/ServiceCollectionExtensionsTests.cs#L13 "void ServiceCollectionExtensionsTests.AddCosmosRepositoryThrowsWithNullServiceCollection()") | 93 | 1 :heavy_check_mark: | 0 | 3 | 4 / 2 |

<a href="#microsoft-azure-cosmosrepositorytests-extensions">:top: back to Microsoft.Azure.CosmosRepositoryTests.Extensions</a>

</details>

</details>

<details>
<summary>
  <strong id="microsoft-azure-cosmosrepositorytests-providers">
    Microsoft.Azure.CosmosRepositoryTests.Providers :heavy_check_mark:
  </strong>
</summary>
<br>

The `Microsoft.Azure.CosmosRepositoryTests.Providers` namespace contains 6 named types.

- 6 named types.
- 167 total lines of source code.
- Approximately 38 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

<details>
<summary>
  <strong id="defaultcosmosclientprovidertests">
    DefaultCosmosClientProviderTests :heavy_check_mark:
  </strong>
</summary>
<br>

- The `DefaultCosmosClientProviderTests` contains 3 members.
- 41 total lines of source code.
- Approximately 12 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [36](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosClientProviderTests.cs#L36 "void DefaultCosmosClientProviderTests.DefaultCosmosClientProviderCorrectlyDisposesOverload()") | 70 | 1 :heavy_check_mark: | 0 | 7 | 19 / 8 |
| Method | [17](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosClientProviderTests.cs#L17 "void DefaultCosmosClientProviderTests.NewDefaultCosmosClientProviderThrowsWithNullCosmosClientOptionsProvider()") | 85 | 1 :heavy_check_mark: | 0 | 4 | 11 / 2 |
| Method | [29](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosClientProviderTests.cs#L29 "void DefaultCosmosClientProviderTests.NewDefaultCosmosClientProviderThrowsWithNullRepositoryOptionsOverload()") | 93 | 1 :heavy_check_mark: | 0 | 5 | 6 / 2 |

<a href="#microsoft-azure-cosmosrepositorytests-providers">:top: back to Microsoft.Azure.CosmosRepositoryTests.Providers</a>

</details>

<details>
<summary>
  <strong id="defaultcosmoscontainerprovidertests">
    DefaultCosmosContainerProviderTests :heavy_check_mark:
  </strong>
</summary>
<br>

- The `DefaultCosmosContainerProviderTests` contains 8 members.
- 88 total lines of source code.
- Approximately 15 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [18](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosContainerProviderTests.cs#L18 "ILoggerFactory DefaultCosmosContainerProviderTests._loggerFactory") | 93 | 0 :heavy_check_mark: | 0 | 2 | 1 / 1 |
| Method | [57](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosContainerProviderTests.cs#L57 "void DefaultCosmosContainerProviderTests.NewDefaultCosmosContainerProviderThrowsWithNullConnectionString()") | 87 | 1 :heavy_check_mark: | 0 | 7 | 11 / 2 |
| Method | [80](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosContainerProviderTests.cs#L80 "void DefaultCosmosContainerProviderTests.NewDefaultCosmosContainerProviderThrowsWithNullContainerId()") | 87 | 1 :heavy_check_mark: | 0 | 7 | 11 / 2 |
| Method | [21](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosContainerProviderTests.cs#L21 "void DefaultCosmosContainerProviderTests.NewDefaultCosmosContainerProviderThrowsWithNullCosmosClient()") | 84 | 1 :heavy_check_mark: | 0 | 7 | 13 / 2 |
| Method | [35](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosContainerProviderTests.cs#L35 "void DefaultCosmosContainerProviderTests.NewDefaultCosmosContainerProviderThrowsWithNullCosmosPartitionKeyNameProvider()") | 85 | 1 :heavy_check_mark: | 0 | 7 | 12 / 2 |
| Method | [68](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosContainerProviderTests.cs#L68 "void DefaultCosmosContainerProviderTests.NewDefaultCosmosContainerProviderThrowsWithNullDatabaseId()") | 87 | 1 :heavy_check_mark: | 0 | 7 | 11 / 2 |
| Method | [92](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosContainerProviderTests.cs#L92 "void DefaultCosmosContainerProviderTests.NewDefaultCosmosContainerProviderThrowsWithNullLogger()") | 85 | 1 :heavy_check_mark: | 0 | 7 | 12 / 2 |
| Method | [48](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosContainerProviderTests.cs#L48 "void DefaultCosmosContainerProviderTests.NewDefaultCosmosContainerProviderThrowsWithNullOptions()") | 90 | 1 :heavy_check_mark: | 0 | 7 | 8 / 2 |

<a href="#microsoft-azure-cosmosrepositorytests-providers">:top: back to Microsoft.Azure.CosmosRepositoryTests.Providers</a>

</details>

<details>
<summary>
  <strong id="defaultcosmospartitionkeypathprovidertests">
    DefaultCosmosPartitionKeyPathProviderTests :heavy_check_mark:
  </strong>
</summary>
<br>

- The `DefaultCosmosPartitionKeyPathProviderTests` contains 1 members.
- 12 total lines of source code.
- Approximately 4 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [15](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosPartitionKeyPathProviderTests.cs#L15 "void DefaultCosmosPartitionKeyPathProviderTests.CosmosPartitionKeyPathProviderCorrectlyGetsPath()") | 78 | 1 :heavy_check_mark: | 0 | 6 | 9 / 4 |

<a href="#microsoft-azure-cosmosrepositorytests-providers">:top: back to Microsoft.Azure.CosmosRepositoryTests.Providers</a>

</details>

<details>
<summary>
  <strong id="picklechipsitem">
    PickleChipsItem :heavy_check_mark:
  </strong>
</summary>
<br>

- The `PickleChipsItem` contains 2 members.
- 8 total lines of source code.
- Approximately 6 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [31](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosPartitionKeyPathProviderTests.cs#L31 "string PickleChipsItem.GetPartitionKeyValue()") | 100 | 1 :heavy_check_mark: | 0 | 0 | 1 / 1 |
| Property | [29](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosPartitionKeyPathProviderTests.cs#L29 "string PickleChipsItem.PartitionBits") | 100 | 2 :heavy_check_mark: | 0 | 2 | 2 / 3 |

<a href="#microsoft-azure-cosmosrepositorytests-providers">:top: back to Microsoft.Azure.CosmosRepositoryTests.Providers</a>

</details>

<details>
<summary>
  <strong id="testcosmosclientprovider">
    TestCosmosClientProvider :heavy_check_mark:
  </strong>
</summary>
<br>

- The `TestCosmosClientProvider` contains 1 members.
- 5 total lines of source code.
- Approximately 1 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [109](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosContainerProviderTests.cs#L109 "Task<T> TestCosmosClientProvider.UseClientAsync<T>(Func<CosmosClient, Task<T>> consume)") | 100 | 1 :heavy_check_mark: | 0 | 4 | 2 / 1 |

<a href="#microsoft-azure-cosmosrepositorytests-providers">:top: back to Microsoft.Azure.CosmosRepositoryTests.Providers</a>

</details>

<details>
<summary>
  <strong id="testitem">
    TestItem :question:
  </strong>
</summary>
<br>

- The `TestItem` contains 0 members.
- 1 total lines of source code.
- Approximately 0 lines of executable code.
- The highest cyclomatic complexity is 0 :question:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |

<a href="#microsoft-azure-cosmosrepositorytests-providers">:top: back to Microsoft.Azure.CosmosRepositoryTests.Providers</a>

</details>

</details>

<a href="#microsoft-azure-cosmosrepositorytests">:top: back to Microsoft.Azure.CosmosRepositoryTests</a>

## Metric definitions

  - **Maintainability index**: Measures ease of code maintenance. Higher values are better.
  - **Cyclomatic complexity**: Measures the number of branches. Lower values are better.
  - **Depth of inheritance**: Measures length of object inheritance hierarchy. Lower values are better.
  - **Class coupling**: Measures the number of classes that are referenced. Lower values are better.
  - **Lines of source code**: Exact number of lines of source code. Lower values are better.
  - **Lines of executable code**: Approximates the lines of executable code. Lower values are better.

*This file is maintained by a bot.*

<!-- markdownlint-restore -->
