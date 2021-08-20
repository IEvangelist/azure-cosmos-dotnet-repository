<!-- markdownlint-capture -->
<!-- markdownlint-disable -->

# Code Metrics

This file is dynamically maintained by a bot, *please do not* edit this by hand. It represents various [code metrics](https://aka.ms/dotnet/code-metrics), such as cyclomatic complexity, maintainability index, and so on.

<div id='azurefunctiontier'></div>

## AzureFunctionTier :heavy_check_mark:

The *AzureFunctionTier.csproj* project file contains:

- 2 namespaces.
- 4 named types.
- 129 total lines of source code.
- Approximately 39 lines of executable code.
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
- 74 total lines of source code.
- Approximately 21 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

<details>
<summary>
  <strong id="startup">
    Startup :heavy_check_mark:
  </strong>
</summary>
<br>

- The `Startup` contains 1 members.
- 12 total lines of source code.
- Approximately 4 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [11](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/AzureFunctionTier/Startup.cs#L11 "void Startup.Configure(IFunctionsHostBuilder builder)") | 78 | 1 :heavy_check_mark: | 0 | 2 | 9 / 4 |

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
- 55 total lines of source code.
- Approximately 18 lines of executable code.
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

- The `User` contains 6 members.
- 22 total lines of source code.
- Approximately 11 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Property | [21](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/AzureFunctionTier/Model/User.cs#L21 "string User.EmailAddress") | 100 | 2 :heavy_check_mark: | 0 | 1 | 2 / 2 |
| Property | [12](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/AzureFunctionTier/Model/User.cs#L12 "string User.FirstName") | 100 | 2 :heavy_check_mark: | 0 | 1 | 2 / 2 |
| Property | [18](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/AzureFunctionTier/Model/User.cs#L18 "string User.FullName") | 95 | 2 :heavy_check_mark: | 0 | 1 | 2 / 2 |
| Method | [23](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/AzureFunctionTier/Model/User.cs#L23 "string User.GetPartitionKeyValue()") | 100 | 1 :heavy_check_mark: | 0 | 0 | 4 / 1 |
| Property | [15](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/AzureFunctionTier/Model/User.cs#L15 "string User.LastName") | 100 | 2 :heavy_check_mark: | 0 | 1 | 2 / 2 |
| Property | [9](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/AzureFunctionTier/Model/User.cs#L9 "string User.Nickname") | 100 | 2 :heavy_check_mark: | 0 | 1 | 2 / 2 |

<a href="#azurefunctiontier-model">:top: back to AzureFunctionTier.Model</a>

</details>

</details>

<a href="#azurefunctiontier">:top: back to AzureFunctionTier</a>

<div id='inmemorywebtier'></div>

## InMemoryWebTier :heavy_check_mark:

The *InMemoryWebTier.csproj* project file contains:

- 4 namespaces.
- 5 named types.
- 104 total lines of source code.
- Approximately 36 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

<details>
<summary>
  <strong id="global+namespace">
    &lt;global namespace&gt; :heavy_check_mark:
  </strong>
</summary>
<br>

The `<global namespace>` namespace contains 1 named types.

- 1 named types.
- 9 total lines of source code.
- Approximately 8 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

<details>
<summary>
  <strong id="program$">
    &lt;Program&gt;$ :heavy_check_mark:
  </strong>
</summary>
<br>

- The `<Program>$` contains 1 members.
- 9 total lines of source code.
- Approximately 8 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [1](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/InMemoryWebTier/Program.cs#L1 "<top-level-statements-entry-point>") | 80 | 1 :heavy_check_mark: | 0 | 2 | 9 / 4 |

<a href="#global+namespace">:top: back to &lt;global namespace&gt;</a>

</details>

</details>

<details>
<summary>
  <strong id="inmemorywebtier-controllers">
    InMemoryWebTier.Controllers :heavy_check_mark:
  </strong>
</summary>
<br>

The `InMemoryWebTier.Controllers` namespace contains 1 named types.

- 1 named types.
- 34 total lines of source code.
- Approximately 14 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

<details>
<summary>
  <strong id="parcelcontroller">
    ParcelController :heavy_check_mark:
  </strong>
</summary>
<br>

- The `ParcelController` contains 7 members.
- 31 total lines of source code.
- Approximately 14 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [17](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/InMemoryWebTier/Controllers/ParcelController.cs#L17 "IRepository<Parcel> ParcelController._repository") | 100 | 0 :heavy_check_mark: | 0 | 2 | 1 / 0 |
| Method | [19](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/InMemoryWebTier/Controllers/ParcelController.cs#L19 "ParcelController.ParcelController(IRepository<Parcel> repository)") | 96 | 1 :heavy_check_mark: | 0 | 2 | 4 / 1 |
| Method | [33](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/InMemoryWebTier/Controllers/ParcelController.cs#L33 "ValueTask<IEnumerable<Parcel>> ParcelController.CreateParcels(params Parcel[] parcels)") | 87 | 1 :heavy_check_mark: | 0 | 7 | 3 / 2 |
| Method | [41](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/InMemoryWebTier/Controllers/ParcelController.cs#L41 "ValueTask ParcelController.DeleteParcel(string id)") | 86 | 1 :heavy_check_mark: | 0 | 5 | 3 / 2 |
| Method | [29](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/InMemoryWebTier/Controllers/ParcelController.cs#L29 "ValueTask<Parcel> ParcelController.GetParcel(string id)") | 86 | 1 :heavy_check_mark: | 0 | 5 | 3 / 2 |
| Method | [25](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/InMemoryWebTier/Controllers/ParcelController.cs#L25 "ValueTask<IEnumerable<Parcel>> ParcelController.GetParcels()") | 81 | 1 :heavy_check_mark: | 0 | 6 | 3 / 3 |
| Method | [37](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/InMemoryWebTier/Controllers/ParcelController.cs#L37 "ValueTask<Parcel> ParcelController.UpdateParcel(Parcel parcel)") | 87 | 1 :heavy_check_mark: | 0 | 6 | 3 / 2 |

<a href="#inmemorywebtier-controllers">:top: back to InMemoryWebTier.Controllers</a>

</details>

</details>

<details>
<summary>
  <strong id="inmemorywebtier">
    InMemoryWebTier :heavy_check_mark:
  </strong>
</summary>
<br>

The `InMemoryWebTier` namespace contains 1 named types.

- 1 named types.
- 37 total lines of source code.
- Approximately 14 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

<details>
<summary>
  <strong id="startup">
    Startup :heavy_check_mark:
  </strong>
</summary>
<br>

- The `Startup` contains 2 members.
- 34 total lines of source code.
- Approximately 14 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [36](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/InMemoryWebTier/Startup.cs#L36 "void Startup.Configure(IApplicationBuilder app, IWebHostEnvironment env)") | 69 | 1 :heavy_check_mark: | 0 | 3 | 13 / 9 |
| Method | [19](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/InMemoryWebTier/Startup.cs#L19 "void Startup.ConfigureServices(IServiceCollection services)") | 75 | 1 :heavy_check_mark: | 0 | 4 | 17 / 5 |

<a href="#inmemorywebtier">:top: back to InMemoryWebTier</a>

</details>

</details>

<details>
<summary>
  <strong id="inmemorywebtier-models">
    InMemoryWebTier.Models :heavy_check_mark:
  </strong>
</summary>
<br>

The `InMemoryWebTier.Models` namespace contains 2 named types.

- 2 named types.
- 24 total lines of source code.
- Approximately 0 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

<details>
<summary>
  <strong id="parcel">
    Parcel :heavy_check_mark:
  </strong>
</summary>
<br>

- The `Parcel` contains 3 members.
- 8 total lines of source code.
- Approximately 0 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Property | [12](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/InMemoryWebTier/Models/Parcel.cs#L12 "Guid Parcel.CustomerId") | 100 | 2 :heavy_check_mark: | 0 | 1 | 1 / 0 |
| Property | [16](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/InMemoryWebTier/Models/Parcel.cs#L16 "List<ParcelItem> Parcel.Items") | 100 | 2 :heavy_check_mark: | 0 | 2 | 1 / 0 |
| Property | [14](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/InMemoryWebTier/Models/Parcel.cs#L14 "DateTime Parcel.PromisedBy") | 100 | 2 :heavy_check_mark: | 0 | 1 | 1 / 0 |

<a href="#inmemorywebtier-models">:top: back to InMemoryWebTier.Models</a>

</details>

<details>
<summary>
  <strong id="parcelitem">
    ParcelItem :heavy_check_mark:
  </strong>
</summary>
<br>

- The `ParcelItem` contains 3 members.
- 8 total lines of source code.
- Approximately 0 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Property | [12](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/InMemoryWebTier/Models/ParcelItem.cs#L12 "string ParcelItem.Description") | 100 | 2 :heavy_check_mark: | 0 | 0 | 1 / 0 |
| Property | [10](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/InMemoryWebTier/Models/ParcelItem.cs#L10 "string ParcelItem.Sku") | 100 | 2 :heavy_check_mark: | 0 | 0 | 1 / 0 |
| Property | [8](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/InMemoryWebTier/Models/ParcelItem.cs#L8 "string ParcelItem.Upos") | 100 | 2 :heavy_check_mark: | 0 | 0 | 1 / 0 |

<a href="#inmemorywebtier-models">:top: back to InMemoryWebTier.Models</a>

</details>

</details>

<a href="#inmemorywebtier">:top: back to InMemoryWebTier</a>

<div id='servicetier'></div>

## ServiceTier :heavy_check_mark:

The *ServiceTier.csproj* project file contains:

- 1 namespaces.
- 5 named types.
- 299 total lines of source code.
- Approximately 108 lines of executable code.
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
- 299 total lines of source code.
- Approximately 108 lines of executable code.
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
- 215 total lines of source code.
- Approximately 86 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [37](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/Program.cs#L37 "IHostBuilder Program.CreateHostBuilder(string[] args)") | 65 | 1 :heavy_check_mark: | 0 | 2 | 21 / 10 |
| Method | [19](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/Program.cs#L19 "Task Program.Main(string[] args)") | 74 | 1 :heavy_check_mark: | 0 | 5 | 17 / 5 |
| Method | [59](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/Program.cs#L59 "Task Program.RawRepositoryExampleAsync(IRepository<Person> repository)") | 52 | 2 :heavy_check_mark: | 0 | 6 | 49 / 20 |
| Method | [109](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/Program.cs#L109 "Task Program.RawRepositoryExampleAsync(IRepository<Widget> repository)") | 48 | 2 :heavy_check_mark: | 0 | 8 | 72 / 31 |
| Method | [182](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/ServiceTier/Program.cs#L182 "Task Program.ServiceExampleAsync(IExampleService service)") | 52 | 2 :heavy_check_mark: | 0 | 8 | 49 / 20 |

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

<div id='webtier-integration-tests'></div>

## WebTier.Integration.Tests :heavy_check_mark:

The *WebTier.Integration.Tests.csproj* project file contains:

- 2 namespaces.
- 3 named types.
- 86 total lines of source code.
- Approximately 17 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

<details>
<summary>
  <strong id="webtier-integration-tests-factories">
    WebTier.Integration.Tests.Factories :heavy_check_mark:
  </strong>
</summary>
<br>

The `WebTier.Integration.Tests.Factories` namespace contains 1 named types.

- 1 named types.
- 16 total lines of source code.
- Approximately 3 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

<details>
<summary>
  <strong id="webtierapplicationfactory">
    WebTierApplicationFactory :heavy_check_mark:
  </strong>
</summary>
<br>

- The `WebTierApplicationFactory` contains 1 members.
- 13 total lines of source code.
- Approximately 3 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [16](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/WebTier.Integration.Tests/Factories/WebTierApplicationFactory.cs#L16 "void WebTierApplicationFactory.ConfigureWebHost(IWebHostBuilder builder)") | 86 | 1 :heavy_check_mark: | 0 | 4 | 10 / 3 |

<a href="#webtier-integration-tests-factories">:top: back to WebTier.Integration.Tests.Factories</a>

</details>

</details>

<details>
<summary>
  <strong id="webtier-integration-tests">
    WebTier.Integration.Tests :heavy_check_mark:
  </strong>
</summary>
<br>

The `WebTier.Integration.Tests` namespace contains 2 named types.

- 2 named types.
- 70 total lines of source code.
- Approximately 14 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

<details>
<summary>
  <strong id="languagedto">
    LanguageDto :heavy_check_mark:
  </strong>
</summary>
<br>

- The `LanguageDto` contains 6 members.
- 14 total lines of source code.
- Approximately 0 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Property | [15](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/WebTier.Integration.Tests/Models/LanguageDto.cs#L15 "string[] LanguageDto.Aliases") | 100 | 2 :heavy_check_mark: | 0 | 0 | 1 / 0 |
| Property | [17](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/WebTier.Integration.Tests/Models/LanguageDto.cs#L17 "string LanguageDto.Description") | 100 | 2 :heavy_check_mark: | 0 | 0 | 1 / 0 |
| Property | [11](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/WebTier.Integration.Tests/Models/LanguageDto.cs#L11 "string LanguageDto.Id") | 100 | 2 :heavy_check_mark: | 0 | 0 | 1 / 0 |
| Property | [21](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/WebTier.Integration.Tests/Models/LanguageDto.cs#L21 "DateTime LanguageDto.InitialReleaseDate") | 100 | 2 :heavy_check_mark: | 0 | 1 | 1 / 0 |
| Property | [13](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/WebTier.Integration.Tests/Models/LanguageDto.cs#L13 "string LanguageDto.Name") | 100 | 2 :heavy_check_mark: | 0 | 0 | 1 / 0 |
| Property | [19](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/WebTier.Integration.Tests/Models/LanguageDto.cs#L19 "ProgrammingStyle LanguageDto.PrimaryStyle") | 100 | 2 :heavy_check_mark: | 0 | 1 | 1 / 0 |

<a href="#webtier-integration-tests">:top: back to WebTier.Integration.Tests</a>

</details>

<details>
<summary>
  <strong id="languagescontrollertests">
    LanguagesControllerTests :heavy_check_mark:
  </strong>
</summary>
<br>

- The `LanguagesControllerTests` contains 4 members.
- 50 total lines of source code.
- Approximately 14 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [23](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/WebTier.Integration.Tests/LanguagesControllerTests.cs#L23 "HttpClient LanguagesControllerTests._client") | 100 | 0 :heavy_check_mark: | 0 | 1 | 1 / 0 |
| Field | [24](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/WebTier.Integration.Tests/LanguagesControllerTests.cs#L24 "IRepositoryFactory LanguagesControllerTests._repositoryFactory") | 100 | 0 :heavy_check_mark: | 0 | 1 | 1 / 0 |
| Method | [26](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/WebTier.Integration.Tests/LanguagesControllerTests.cs#L26 "LanguagesControllerTests.LanguagesControllerTests(WebTierApplicationFactory factory)") | 86 | 1 :heavy_check_mark: | 0 | 4 | 5 / 2 |
| Method | [33](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository.Samples/WebTier.Integration.Tests/LanguagesControllerTests.cs#L33 "Task LanguagesControllerTests.Post_Always_Creates_A_Language()") | 59 | 1 :heavy_check_mark: | 0 | 14 | 38 / 12 |

<a href="#webtier-integration-tests">:top: back to WebTier.Integration.Tests</a>

</details>

</details>

<a href="#webtier-integration-tests">:top: back to WebTier.Integration.Tests</a>

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

- 8 namespaces.
- 34 named types.
- 1,667 total lines of source code.
- Approximately 375 lines of executable code.
- The highest cyclomatic complexity is 10 :radioactive:.

<details>
<summary>
  <strong id="microsoft-azure-cosmosrepository-attributes">
    Microsoft.Azure.CosmosRepository.Attributes :heavy_check_mark:
  </strong>
</summary>
<br>

The `Microsoft.Azure.CosmosRepository.Attributes` namespace contains 3 named types.

- 3 named types.
- 75 total lines of source code.
- Approximately 11 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

<details>
<summary>
  <strong id="containerattribute">
    ContainerAttribute :heavy_check_mark:
  </strong>
</summary>
<br>

- The `ContainerAttribute` contains 2 members.
- 20 total lines of source code.
- Approximately 3 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [26](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Attributes/ContainerAttribute.cs#L26 "ContainerAttribute.ContainerAttribute(string name)") | 91 | 2 :heavy_check_mark: | 0 | 3 | 6 / 1 |
| Property | [20](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Attributes/ContainerAttribute.cs#L20 "string ContainerAttribute.Name") | 100 | 1 :heavy_check_mark: | 0 | 0 | 4 / 0 |

<a href="#microsoft-azure-cosmosrepository-attributes">:top: back to Microsoft.Azure.CosmosRepository.Attributes</a>

</details>

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

<details>
<summary>
  <strong id="uniquekeyattribute">
    UniqueKeyAttribute :heavy_check_mark:
  </strong>
</summary>
<br>

- The `UniqueKeyAttribute` contains 2 members.
- 20 total lines of source code.
- Approximately 4 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [25](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Attributes/UniqueKeyAttribute.cs#L25 "UniqueKeyAttribute.UniqueKeyAttribute(string keyName)") | 91 | 2 :heavy_check_mark: | 0 | 3 | 6 / 1 |
| Property | [19](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Attributes/UniqueKeyAttribute.cs#L19 "string UniqueKeyAttribute.KeyName") | 100 | 1 :heavy_check_mark: | 0 | 0 | 4 / 1 |

<a href="#microsoft-azure-cosmosrepository-attributes">:top: back to Microsoft.Azure.CosmosRepository.Attributes</a>

</details>

</details>

<details>
<summary>
  <strong id="microsoft-azure-cosmosrepository-builders">
    Microsoft.Azure.CosmosRepository.Builders :heavy_check_mark:
  </strong>
</summary>
<br>

The `Microsoft.Azure.CosmosRepository.Builders` namespace contains 3 named types.

- 3 named types.
- 96 total lines of source code.
- Approximately 13 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

<details>
<summary>
  <strong id="containeroptionsbuilder">
    ContainerOptionsBuilder :heavy_check_mark:
  </strong>
</summary>
<br>

- The `ContainerOptionsBuilder` contains 6 members.
- 50 total lines of source code.
- Approximately 5 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [22](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Builders/ContainerOptionsBuilder.cs#L22 "ContainerOptionsBuilder.ContainerOptionsBuilder(Type type)") | 96 | 1 :heavy_check_mark: | 0 | 1 | 5 / 1 |
| Property | [27](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Builders/ContainerOptionsBuilder.cs#L27 "string ContainerOptionsBuilder.Name") | 100 | 2 :heavy_check_mark: | 0 | 0 | 4 / 0 |
| Property | [32](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Builders/ContainerOptionsBuilder.cs#L32 "string ContainerOptionsBuilder.PartitionKey") | 100 | 2 :heavy_check_mark: | 0 | 0 | 4 / 0 |
| Property | [16](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Builders/ContainerOptionsBuilder.cs#L16 "Type ContainerOptionsBuilder.Type") | 100 | 1 :heavy_check_mark: | 0 | 1 | 4 / 0 |
| Method | [40](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Builders/ContainerOptionsBuilder.cs#L40 "ContainerOptionsBuilder ContainerOptionsBuilder.WithContainer(string name)") | 85 | 2 :heavy_check_mark: | 0 | 3 | 11 / 2 |
| Method | [52](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Builders/ContainerOptionsBuilder.cs#L52 "ContainerOptionsBuilder ContainerOptionsBuilder.WithPartitionKey(string partitionKey)") | 85 | 2 :heavy_check_mark: | 0 | 3 | 11 / 2 |

<a href="#microsoft-azure-cosmosrepository-builders">:top: back to Microsoft.Azure.CosmosRepository.Builders</a>

</details>

<details>
<summary>
  <strong id="defaultitemcontainerbuilder">
    DefaultItemContainerBuilder :heavy_check_mark:
  </strong>
</summary>
<br>

- The `DefaultItemContainerBuilder` contains 3 members.
- 20 total lines of source code.
- Approximately 8 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [14](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Builders/DefaultItemContainerBuilder.cs#L14 "List<ContainerOptionsBuilder> DefaultItemContainerBuilder._options") | 93 | 0 :heavy_check_mark: | 0 | 2 | 1 / 1 |
| Method | [18](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Builders/DefaultItemContainerBuilder.cs#L18 "IItemContainerBuilder DefaultItemContainerBuilder.Configure<TItem>(Action<ContainerOptionsBuilder> containerOptions)") | 74 | 2 :heavy_check_mark: | 0 | 8 | 12 / 5 |
| Property | [16](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Builders/DefaultItemContainerBuilder.cs#L16 "IReadOnlyList<ContainerOptionsBuilder> DefaultItemContainerBuilder.Options") | 100 | 2 :heavy_check_mark: | 0 | 3 | 1 / 2 |

<a href="#microsoft-azure-cosmosrepository-builders">:top: back to Microsoft.Azure.CosmosRepository.Builders</a>

</details>

<details>
<summary>
  <strong id="iitemcontainerbuilder">
    IItemContainerBuilder :heavy_check_mark:
  </strong>
</summary>
<br>

- The `IItemContainerBuilder` contains 2 members.
- 17 total lines of source code.
- Approximately 0 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [26](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Builders/IItemContainerBuilder.cs#L26 "IItemContainerBuilder IItemContainerBuilder.Configure<TItem>(Action<ContainerOptionsBuilder> containerOptions)") | 100 | 1 :heavy_check_mark: | 0 | 3 | 6 / 0 |
| Property | [19](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Builders/IItemContainerBuilder.cs#L19 "IReadOnlyList<ContainerOptionsBuilder> IItemContainerBuilder.Options") | 100 | 1 :heavy_check_mark: | 0 | 2 | 4 / 0 |

<a href="#microsoft-azure-cosmosrepository-builders">:top: back to Microsoft.Azure.CosmosRepository.Builders</a>

</details>

</details>

<details>
<summary>
  <strong id="microsoft-azure-cosmosrepository">
    Microsoft.Azure.CosmosRepository :heavy_check_mark:
  </strong>
</summary>
<br>

The `Microsoft.Azure.CosmosRepository` namespace contains 7 named types.

- 7 named types.
- 726 total lines of source code.
- Approximately 209 lines of executable code.
- The highest cyclomatic complexity is 6 :heavy_check_mark:.

<details>
<summary>
  <strong id="defaultrepositorytitem">
    DefaultRepository&lt;TItem&gt; :heavy_check_mark:
  </strong>
</summary>
<br>

- The `DefaultRepository<TItem>` contains 21 members.
- 262 total lines of source code.
- Approximately 97 lines of executable code.
- The highest cyclomatic complexity is 4 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [25](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L25 "ICosmosContainerProvider<TItem> DefaultRepository<TItem>._containerProvider") | 100 | 0 :heavy_check_mark: | 0 | 1 | 1 / 0 |
| Field | [27](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L27 "ILogger<DefaultRepository<TItem>> DefaultRepository<TItem>._logger") | 100 | 0 :heavy_check_mark: | 0 | 1 | 1 / 0 |
| Field | [26](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L26 "IOptionsMonitor<RepositoryOptions> DefaultRepository<TItem>._optionsMonitor") | 100 | 0 :heavy_check_mark: | 0 | 2 | 1 / 0 |
| Method | [35](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L35 "DefaultRepository<TItem>.DefaultRepository(IOptionsMonitor<RepositoryOptions> optionsMonitor, ICosmosContainerProvider<TItem> containerProvider, ILogger<DefaultRepository<TItem>> logger)") | 89 | 1 :heavy_check_mark: | 0 | 5 | 5 / 1 |
| Method | [130](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L130 "ValueTask<TItem> DefaultRepository<TItem>.CreateAsync(TItem value, CancellationToken cancellationToken = null)") | 68 | 1 :heavy_check_mark: | 0 | 9 | 16 / 6 |
| Method | [147](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L147 "ValueTask<IEnumerable<TItem>> DefaultRepository<TItem>.CreateAsync(IEnumerable<TItem> values, CancellationToken cancellationToken = null)") | 71 | 1 :heavy_check_mark: | 0 | 5 | 13 / 6 |
| Method | [178](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L178 "ValueTask DefaultRepository<TItem>.DeleteAsync(TItem value, CancellationToken cancellationToken = null)") | 80 | 1 :heavy_check_mark: | 0 | 2 | 5 / 2 |
| Method | [184](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L184 "ValueTask DefaultRepository<TItem>.DeleteAsync(string id, string partitionKeyValue = null, CancellationToken cancellationToken = null)") | 76 | 2 :heavy_check_mark: | 0 | 4 | 6 / 3 |
| Method | [191](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L191 "ValueTask DefaultRepository<TItem>.DeleteAsync(string id, PartitionKey partitionKey, CancellationToken cancellationToken = null)") | 65 | 2 :heavy_check_mark: | 0 | 10 | 19 / 8 |
| Method | [211](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L211 "ValueTask<bool> DefaultRepository<TItem>.ExistsAsync(string id, string partitionKeyValue = null, CancellationToken cancellationToken = null)") | 76 | 2 :heavy_check_mark: | 0 | 4 | 3 / 3 |
| Method | [215](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L215 "ValueTask<bool> DefaultRepository<TItem>.ExistsAsync(string id, PartitionKey partitionKey, CancellationToken cancellationToken = null)") | 65 | 2 :heavy_check_mark: | 0 | 9 | 25 / 8 |
| Method | [241](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L241 "ValueTask<bool> DefaultRepository<TItem>.ExistsAsync(Expression<Func<TItem, bool>> predicate, CancellationToken cancellationToken = null)") | 64 | 2 :heavy_check_mark: | 0 | 11 | 17 / 8 |
| Method | [42](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L42 "ValueTask<TItem> DefaultRepository<TItem>.GetAsync(string id, string partitionKeyValue = null, CancellationToken cancellationToken = null)") | 76 | 2 :heavy_check_mark: | 0 | 4 | 6 / 3 |
| Method | [49](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L49 "ValueTask<TItem> DefaultRepository<TItem>.GetAsync(string id, PartitionKey partitionKey, CancellationToken cancellationToken = null)") | 62 | 4 :heavy_check_mark: | 0 | 10 | 24 / 9 |
| Method | [74](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L74 "ValueTask<IEnumerable<TItem>> DefaultRepository<TItem>.GetAsync(Expression<Func<TItem, bool>> predicate, CancellationToken cancellationToken = null)") | 60 | 4 :heavy_check_mark: | 0 | 14 | 28 / 12 |
| Method | [103](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L103 "ValueTask<IEnumerable<TItem>> DefaultRepository<TItem>.GetByQueryAsync(string query, CancellationToken cancellationToken = null)") | 69 | 1 :heavy_check_mark: | 0 | 9 | 13 / 6 |
| Method | [117](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L117 "ValueTask<IEnumerable<TItem>> DefaultRepository<TItem>.GetByQueryAsync(QueryDefinition queryDefinition, CancellationToken cancellationToken = null)") | 71 | 1 :heavy_check_mark: | 0 | 9 | 12 / 5 |
| Method | [258](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L258 "Task<IEnumerable<TItem>> DefaultRepository<TItem>.IterateQueryInternalAsync(Container container, QueryDefinition queryDefinition, CancellationToken cancellationToken)") | 71 | 2 :heavy_check_mark: | 0 | 9 | 16 / 6 |
| Property | [29](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L29 "(bool OptimizeBandwidth, ItemRequestOptions Options) DefaultRepository<TItem>.RequestOptions") | 100 | 2 :heavy_check_mark: | 0 | 5 | 5 / 2 |
| Method | [276](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/DefaultRepository.cs#L276 "void DefaultRepository<TItem>.TryLogDebugDetails(ILogger logger, Func<string> getMessage)") | 86 | 4 :heavy_check_mark: | 0 | 3 | 7 / 2 |
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
| Property | [14](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/IItem.cs#L14 "string IItem.Id") | 100 | 2 :heavy_check_mark: | 0 | 0 | 4 / 0 |
| Property | [24](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/IItem.cs#L24 "string IItem.PartitionKey") | 100 | 1 :heavy_check_mark: | 0 | 0 | 4 / 0 |
| Property | [19](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/IItem.cs#L19 "string IItem.Type") | 100 | 2 :heavy_check_mark: | 0 | 0 | 4 / 0 |

<a href="#microsoft-azure-cosmosrepository">:top: back to Microsoft.Azure.CosmosRepository</a>

</details>

<details>
<summary>
  <strong id="inmemoryrepositorytitem">
    InMemoryRepository&lt;TItem&gt; :heavy_check_mark:
  </strong>
</summary>
<br>

- The `InMemoryRepository<TItem>` contains 17 members.
- 142 total lines of source code.
- Approximately 67 lines of executable code.
- The highest cyclomatic complexity is 6 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [157](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/InMemoryRepository.cs#L157 "void InMemoryRepository<TItem>.Conflict()") | 100 | 1 :heavy_check_mark: | 0 | 3 | 1 / 1 |
| Method | [67](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/InMemoryRepository.cs#L67 "ValueTask<TItem> InMemoryRepository<TItem>.CreateAsync(TItem value, CancellationToken cancellationToken = null)") | 64 | 3 :heavy_check_mark: | 0 | 4 | 18 / 9 |
| Method | [86](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/InMemoryRepository.cs#L86 "ValueTask<IEnumerable<TItem>> InMemoryRepository<TItem>.CreateAsync(IEnumerable<TItem> values, CancellationToken cancellationToken = null)") | 73 | 2 :heavy_check_mark: | 0 | 4 | 12 / 5 |
| Method | [109](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/InMemoryRepository.cs#L109 "ValueTask InMemoryRepository<TItem>.DeleteAsync(TItem value, CancellationToken cancellationToken = null)") | 80 | 1 :heavy_check_mark: | 0 | 2 | 3 / 2 |
| Method | [113](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/InMemoryRepository.cs#L113 "ValueTask InMemoryRepository<TItem>.DeleteAsync(string id, string partitionKeyValue = null, CancellationToken cancellationToken = null)") | 77 | 1 :heavy_check_mark: | 0 | 3 | 4 / 3 |
| Method | [118](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/InMemoryRepository.cs#L118 "ValueTask InMemoryRepository<TItem>.DeleteAsync(string id, PartitionKey partitionKey, CancellationToken cancellationToken = null)") | 64 | 4 :heavy_check_mark: | 0 | 6 | 19 / 9 |
| Method | [138](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/InMemoryRepository.cs#L138 "ValueTask<bool> InMemoryRepository<TItem>.ExistsAsync(string id, string partitionKeyValue = null, CancellationToken cancellationToken = null)") | 76 | 2 :heavy_check_mark: | 0 | 4 | 3 / 3 |
| Method | [142](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/InMemoryRepository.cs#L142 "ValueTask<bool> InMemoryRepository<TItem>.ExistsAsync(string id, PartitionKey partitionKey, CancellationToken cancellationToken = null)") | 73 | 2 :heavy_check_mark: | 0 | 5 | 6 / 4 |
| Method | [149](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/InMemoryRepository.cs#L149 "ValueTask<bool> InMemoryRepository<TItem>.ExistsAsync(Expression<Func<TItem, bool>> predicate, CancellationToken cancellationToken = null)") | 76 | 1 :heavy_check_mark: | 0 | 7 | 7 / 4 |
| Method | [23](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/InMemoryRepository.cs#L23 "ValueTask<TItem> InMemoryRepository<TItem>.GetAsync(string id, string partitionKeyValue = null, CancellationToken cancellationToken = null)") | 76 | 2 :heavy_check_mark: | 0 | 4 | 3 / 3 |
| Method | [27](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/InMemoryRepository.cs#L27 "ValueTask<TItem> InMemoryRepository<TItem>.GetAsync(string id, PartitionKey partitionKey, CancellationToken cancellationToken = null)") | 63 | 6 :heavy_check_mark: | 0 | 6 | 19 / 9 |
| Method | [47](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/InMemoryRepository.cs#L47 "ValueTask<IEnumerable<TItem>> InMemoryRepository<TItem>.GetAsync(Expression<Func<TItem, bool>> predicate, CancellationToken cancellationToken = null)") | 76 | 1 :heavy_check_mark: | 0 | 8 | 7 / 4 |
| Method | [55](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/InMemoryRepository.cs#L55 "ValueTask<IEnumerable<TItem>> InMemoryRepository<TItem>.GetByQueryAsync(string query, CancellationToken cancellationToken = null)") | 89 | 1 :heavy_check_mark: | 0 | 5 | 5 / 2 |
| Method | [61](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/InMemoryRepository.cs#L61 "ValueTask<IEnumerable<TItem>> InMemoryRepository<TItem>.GetByQueryAsync(QueryDefinition queryDefinition, CancellationToken cancellationToken = null)") | 89 | 1 :heavy_check_mark: | 0 | 6 | 5 / 2 |
| Property | [20](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/InMemoryRepository.cs#L20 "ConcurrentDictionary<string, TItem> InMemoryRepository<TItem>.Items") | 100 | 1 :heavy_check_mark: | 0 | 1 | 1 / 1 |
| Method | [156](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/InMemoryRepository.cs#L156 "void InMemoryRepository<TItem>.NotFound()") | 100 | 1 :heavy_check_mark: | 0 | 3 | 1 / 1 |
| Method | [99](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/InMemoryRepository.cs#L99 "ValueTask<TItem> InMemoryRepository<TItem>.UpdateAsync(TItem value, CancellationToken cancellationToken = null)") | 70 | 2 :heavy_check_mark: | 0 | 4 | 9 / 5 |

<a href="#microsoft-azure-cosmosrepository">:top: back to Microsoft.Azure.CosmosRepository</a>

</details>

<details>
<summary>
  <strong id="irepositorytitem">
    IRepository&lt;TItem&gt; :heavy_check_mark:
  </strong>
</summary>
<br>

- The `IRepository<TItem>` contains 14 members.
- 182 total lines of source code.
- Approximately 34 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [106](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/IRepository.cs#L106 "ValueTask<TItem> IRepository<TItem>.CreateAsync(TItem value, CancellationToken cancellationToken = null)") | 87 | 1 :heavy_check_mark: | 0 | 2 | 9 / 2 |
| Method | [116](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/IRepository.cs#L116 "ValueTask<IEnumerable<TItem>> IRepository<TItem>.CreateAsync(IEnumerable<TItem> values, CancellationToken cancellationToken = null)") | 87 | 1 :heavy_check_mark: | 0 | 3 | 9 / 2 |
| Method | [136](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/IRepository.cs#L136 "ValueTask IRepository<TItem>.DeleteAsync(TItem value, CancellationToken cancellationToken = null)") | 87 | 1 :heavy_check_mark: | 0 | 2 | 9 / 2 |
| Method | [147](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/IRepository.cs#L147 "ValueTask IRepository<TItem>.DeleteAsync(string id, string partitionKeyValue = null, CancellationToken cancellationToken = null)") | 80 | 1 :heavy_check_mark: | 0 | 2 | 11 / 4 |
| Method | [159](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/IRepository.cs#L159 "ValueTask IRepository<TItem>.DeleteAsync(string id, PartitionKey partitionKey, CancellationToken cancellationToken = null)") | 87 | 1 :heavy_check_mark: | 0 | 3 | 11 / 2 |
| Method | [173](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/IRepository.cs#L173 "ValueTask<bool> IRepository<TItem>.ExistsAsync(string id, string partitionKeyValue = null, CancellationToken cancellationToken = null)") | 80 | 1 :heavy_check_mark: | 0 | 2 | 9 / 4 |
| Method | [183](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/IRepository.cs#L183 "ValueTask<bool> IRepository<TItem>.ExistsAsync(string id, PartitionKey partitionKey, CancellationToken cancellationToken = null)") | 87 | 1 :heavy_check_mark: | 0 | 3 | 9 / 2 |
| Method | [192](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/IRepository.cs#L192 "ValueTask<bool> IRepository<TItem>.ExistsAsync(Expression<Func<TItem, bool>> predicate, CancellationToken cancellationToken = null)") | 87 | 1 :heavy_check_mark: | 0 | 4 | 9 / 2 |
| Method | [44](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/IRepository.cs#L44 "ValueTask<TItem> IRepository<TItem>.GetAsync(string id, string partitionKeyValue = null, CancellationToken cancellationToken = null)") | 80 | 1 :heavy_check_mark: | 0 | 2 | 14 / 4 |
| Method | [59](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/IRepository.cs#L59 "ValueTask<TItem> IRepository<TItem>.GetAsync(string id, PartitionKey partitionKey, CancellationToken cancellationToken = null)") | 87 | 1 :heavy_check_mark: | 0 | 3 | 14 / 2 |
| Method | [74](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/IRepository.cs#L74 "ValueTask<IEnumerable<TItem>> IRepository<TItem>.GetAsync(Expression<Func<TItem, bool>> predicate, CancellationToken cancellationToken = null)") | 87 | 1 :heavy_check_mark: | 0 | 5 | 13 / 2 |
| Method | [85](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/IRepository.cs#L85 "ValueTask<IEnumerable<TItem>> IRepository<TItem>.GetByQueryAsync(string query, CancellationToken cancellationToken = null)") | 87 | 1 :heavy_check_mark: | 0 | 3 | 10 / 2 |
| Method | [96](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/IRepository.cs#L96 "ValueTask<IEnumerable<TItem>> IRepository<TItem>.GetByQueryAsync(QueryDefinition queryDefinition, CancellationToken cancellationToken = null)") | 87 | 1 :heavy_check_mark: | 0 | 4 | 10 / 2 |
| Method | [126](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/IRepository.cs#L126 "ValueTask<TItem> IRepository<TItem>.UpdateAsync(TItem value, CancellationToken cancellationToken = null)") | 87 | 1 :heavy_check_mark: | 0 | 2 | 9 / 2 |

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
| Method | [59](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Item.cs#L59 "Item.Item()") | 100 | 1 :heavy_check_mark: | 0 | 1 | 4 / 1 |
| Method | [68](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Item.cs#L68 "string Item.GetPartitionKeyValue()") | 100 | 1 :heavy_check_mark: | 0 | 0 | 8 / 1 |
| Property | [42](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Item.cs#L42 "string Item.Id") | 100 | 2 :heavy_check_mark: | 0 | 2 | 8 / 3 |
| Property | [54](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Item.cs#L54 "string Item.PartitionKey") | 100 | 2 :heavy_check_mark: | 0 | 1 | 5 / 2 |
| Property | [48](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Item.cs#L48 "string Item.Type") | 100 | 2 :heavy_check_mark: | 0 | 1 | 5 / 2 |

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
- 101 total lines of source code.
- Approximately 22 lines of executable code.
- The highest cyclomatic complexity is 3 :heavy_check_mark:.

<details>
<summary>
  <strong id="servicecollectionextensions">
    ServiceCollectionExtensions :heavy_check_mark:
  </strong>
</summary>
<br>

- The `ServiceCollectionExtensions` contains 4 members.
- 98 total lines of source code.
- Approximately 22 lines of executable code.
- The highest cyclomatic complexity is 3 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [31](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Extensions/ServiceCollectionExtensions.cs#L31 "IServiceCollection ServiceCollectionExtensions.AddCosmosRepository(IServiceCollection services, Action<RepositoryOptions> setupAction = null, Action<CosmosClientOptions> additionSetupAction = null)") | 62 | 3 :heavy_check_mark: | 0 | 9 | 44 / 10 |
| Method | [110](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Extensions/ServiceCollectionExtensions.cs#L110 "IServiceCollection ServiceCollectionExtensions.AddCosmosRepository(IServiceCollection services, IConfiguration configuration, Action<RepositoryOptions> setupAction = null)") | 78 | 1 :heavy_check_mark: | 0 | 7 | 16 / 3 |
| Method | [73](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Extensions/ServiceCollectionExtensions.cs#L73 "IServiceCollection ServiceCollectionExtensions.AddInMemoryCosmosRepository(IServiceCollection services)") | 77 | 2 :heavy_check_mark: | 0 | 5 | 18 / 4 |
| Method | [92](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Extensions/ServiceCollectionExtensions.cs#L92 "IServiceCollection ServiceCollectionExtensions.RemoveCosmosRepositories(IServiceCollection services)") | 75 | 1 :heavy_check_mark: | 0 | 5 | 11 / 5 |

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
- 62 total lines of source code.
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
- 22 total lines of source code.
- Approximately 5 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [11](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Extensions/ParameterRebinder.cs#L11 "Dictionary<ParameterExpression, ParameterExpression> ParameterRebinder._map") | 100 | 0 :heavy_check_mark: | 0 | 2 | 1 / 0 |
| Method | [13](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Extensions/ParameterRebinder.cs#L13 "ParameterRebinder.ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)") | 95 | 2 :heavy_check_mark: | 0 | 3 | 2 / 1 |
| Method | [16](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Extensions/ParameterRebinder.cs#L16 "Expression ParameterRebinder.ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)") | 94 | 1 :heavy_check_mark: | 0 | 4 | 3 / 1 |
| Method | [21](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Extensions/ParameterRebinder.cs#L21 "Expression ParameterRebinder.VisitParameter(ParameterExpression parameter)") | 81 | 2 :heavy_check_mark: | 0 | 5 | 10 / 3 |

<a href="#microsoft-azure-cosmosrepository-extensions">:top: back to Microsoft.Azure.CosmosRepository.Extensions</a>

</details>

</details>

<details>
<summary>
  <strong id="microsoft-azure-cosmosrepository-internals">
    Microsoft.Azure.CosmosRepository.Internals :heavy_check_mark:
  </strong>
</summary>
<br>

The `Microsoft.Azure.CosmosRepository.Internals` namespace contains 1 named types.

- 1 named types.
- 10 total lines of source code.
- Approximately 1 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

<details>
<summary>
  <strong id="cosmosclientoptionsmanipulator">
    CosmosClientOptionsManipulator :heavy_check_mark:
  </strong>
</summary>
<br>

- The `CosmosClientOptionsManipulator` contains 2 members.
- 7 total lines of source code.
- Approximately 1 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [13](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Internals/CosmosClientOptionsManipulator.cs#L13 "CosmosClientOptionsManipulator.CosmosClientOptionsManipulator(Action<CosmosClientOptions> configure)") | 95 | 2 :heavy_check_mark: | 0 | 3 | 2 / 1 |
| Property | [11](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Internals/CosmosClientOptionsManipulator.cs#L11 "Action<CosmosClientOptions> CosmosClientOptionsManipulator.Configure") | 100 | 1 :heavy_check_mark: | 0 | 2 | 1 / 0 |

<a href="#microsoft-azure-cosmosrepository-internals">:top: back to Microsoft.Azure.CosmosRepository.Internals</a>

</details>

</details>

<details>
<summary>
  <strong id="microsoft-azure-cosmosrepository-options">
    Microsoft.Azure.CosmosRepository.Options :heavy_check_mark:
  </strong>
</summary>
<br>

The `Microsoft.Azure.CosmosRepository.Options` namespace contains 3 named types.

- 3 named types.
- 128 total lines of source code.
- Approximately 11 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

<details>
<summary>
  <strong id="itemoptions">
    ItemOptions :heavy_check_mark:
  </strong>
</summary>
<br>

- The `ItemOptions` contains 5 members.
- 18 total lines of source code.
- Approximately 4 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [19](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Options/ItemOptions.cs#L19 "ItemOptions.ItemOptions(Type type, string containerName, string partitionKeyPath, UniqueKeyPolicy uniqueKeyPolicy)") | 75 | 1 :heavy_check_mark: | 0 | 2 | 7 / 4 |
| Property | [13](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Options/ItemOptions.cs#L13 "string ItemOptions.ContainerName") | 100 | 1 :heavy_check_mark: | 0 | 0 | 1 / 0 |
| Property | [15](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Options/ItemOptions.cs#L15 "string ItemOptions.PartitionKeyPath") | 100 | 1 :heavy_check_mark: | 0 | 0 | 1 / 0 |
| Property | [11](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Options/ItemOptions.cs#L11 "Type ItemOptions.Type") | 100 | 1 :heavy_check_mark: | 0 | 1 | 1 / 0 |
| Property | [17](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Options/ItemOptions.cs#L17 "UniqueKeyPolicy ItemOptions.UniqueKeyPolicy") | 100 | 1 :heavy_check_mark: | 0 | 1 | 1 / 0 |

<a href="#microsoft-azure-cosmosrepository-options">:top: back to Microsoft.Azure.CosmosRepository.Options</a>

</details>

<details>
<summary>
  <strong id="repositoryoptions">
    RepositoryOptions :heavy_check_mark:
  </strong>
</summary>
<br>

- The `RepositoryOptions` contains 9 members.
- 76 total lines of source code.
- Approximately 6 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Property | [68](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Options/RepositoryOptions.cs#L68 "bool RepositoryOptions.AllowBulkExecution") | 100 | 2 :heavy_check_mark: | 0 | 0 | 9 / 0 |
| Property | [79](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Options/RepositoryOptions.cs#L79 "IItemContainerBuilder RepositoryOptions.ContainerBuilder") | 100 | 1 :heavy_check_mark: | 0 | 2 | 5 / 1 |
| Property | [35](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Options/RepositoryOptions.cs#L35 "string RepositoryOptions.ContainerId") | 100 | 2 :heavy_check_mark: | 0 | 0 | 7 / 1 |
| Property | [84](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Options/RepositoryOptions.cs#L84 "IReadOnlyList<ContainerOptionsBuilder> RepositoryOptions.ContainerOptions") | 96 | 2 :heavy_check_mark: | 0 | 3 | 4 / 2 |
| Property | [57](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Options/RepositoryOptions.cs#L57 "bool RepositoryOptions.ContainerPerItemType") | 100 | 2 :heavy_check_mark: | 0 | 0 | 9 / 0 |
| Property | [19](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Options/RepositoryOptions.cs#L19 "string RepositoryOptions.CosmosConnectionString") | 100 | 2 :heavy_check_mark: | 0 | 0 | 4 / 0 |
| Property | [27](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Options/RepositoryOptions.cs#L27 "string RepositoryOptions.DatabaseId") | 100 | 2 :heavy_check_mark: | 0 | 0 | 7 / 1 |
| Property | [47](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Options/RepositoryOptions.cs#L47 "bool RepositoryOptions.OptimizeBandwidth") | 100 | 2 :heavy_check_mark: | 0 | 0 | 11 / 1 |
| Property | [73](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Options/RepositoryOptions.cs#L73 "RepositorySerializationOptions RepositoryOptions.SerializationOptions") | 100 | 2 :heavy_check_mark: | 0 | 1 | 4 / 0 |

<a href="#microsoft-azure-cosmosrepository-options">:top: back to Microsoft.Azure.CosmosRepository.Options</a>

</details>

<details>
<summary>
  <strong id="repositoryserializationoptions">
    RepositorySerializationOptions :heavy_check_mark:
  </strong>
</summary>
<br>

- The `RepositorySerializationOptions` contains 3 members.
- 25 total lines of source code.
- Approximately 1 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Property | [18](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Options/RepositorySerializationOptions.cs#L18 "bool RepositorySerializationOptions.IgnoreNullValues") | 100 | 2 :heavy_check_mark: | 0 | 0 | 5 / 0 |
| Property | [24](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Options/RepositorySerializationOptions.cs#L24 "bool RepositorySerializationOptions.Indented") | 100 | 2 :heavy_check_mark: | 0 | 0 | 5 / 0 |
| Property | [31](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Options/RepositorySerializationOptions.cs#L31 "CosmosPropertyNamingPolicy RepositorySerializationOptions.PropertyNamingPolicy") | 100 | 2 :heavy_check_mark: | 0 | 2 | 6 / 1 |

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

The `Microsoft.Azure.CosmosRepository.Providers` namespace contains 14 named types.

- 14 named types.
- 469 total lines of source code.
- Approximately 94 lines of executable code.
- The highest cyclomatic complexity is 10 :radioactive:.

<details>
<summary>
  <strong id="defaultcosmosclientoptionsprovider">
    DefaultCosmosClientOptionsProvider :heavy_check_mark:
  </strong>
</summary>
<br>

- The `DefaultCosmosClientOptionsProvider` contains 5 members.
- 68 total lines of source code.
- Approximately 15 lines of executable code.
- The highest cyclomatic complexity is 7 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [18](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosClientOptionsProvider.cs#L18 "Lazy<CosmosClientOptions> DefaultCosmosClientOptionsProvider._lazyClientOptions") | 100 | 0 :heavy_check_mark: | 0 | 2 | 1 / 0 |
| Method | [28](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosClientOptionsProvider.cs#L28 "DefaultCosmosClientOptionsProvider.DefaultCosmosClientOptionsProvider(IServiceProvider serviceProvider, IConfiguration configuration)") | 85 | 1 :heavy_check_mark: | 0 | 4 | 10 / 2 |
| Method | [66](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosClientOptionsProvider.cs#L66 "HttpClient DefaultCosmosClientOptionsProvider.ClientFactory(IServiceProvider serviceProvider)") | 78 | 1 :heavy_check_mark: | 0 | 3 | 16 / 4 |
| Property | [21](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosClientOptionsProvider.cs#L21 "CosmosClientOptions DefaultCosmosClientOptionsProvider.ClientOptions") | 100 | 2 :heavy_check_mark: | 0 | 3 | 2 / 2 |
| Method | [34](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosClientOptionsProvider.cs#L34 "CosmosClientOptions DefaultCosmosClientOptionsProvider.CreateCosmosClientOptions(IServiceProvider serviceProvider, IConfiguration configuration)") | 65 | 7 :heavy_check_mark: | 0 | 9 | 31 / 7 |

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
  <strong id="defaultcosmoscontainernameprovider">
    DefaultCosmosContainerNameProvider :heavy_check_mark:
  </strong>
</summary>
<br>

- The `DefaultCosmosContainerNameProvider` contains 4 members.
- 32 total lines of source code.
- Approximately 10 lines of executable code.
- The highest cyclomatic complexity is 4 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [17](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosContainerNameProvider.cs#L17 "IOptions<RepositoryOptions> DefaultCosmosContainerNameProvider._options") | 100 | 0 :heavy_check_mark: | 0 | 2 | 1 / 0 |
| Method | [20](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosContainerNameProvider.cs#L20 "DefaultCosmosContainerNameProvider.DefaultCosmosContainerNameProvider(IOptions<RepositoryOptions> options)") | 92 | 2 :heavy_check_mark: | 0 | 5 | 4 / 1 |
| Field | [18](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosContainerNameProvider.cs#L18 "ConcurrentDictionary<Type, string> DefaultCosmosContainerNameProvider.ContainerNameMap") | 93 | 0 :heavy_check_mark: | 0 | 2 | 1 / 1 |
| Method | [26](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosContainerNameProvider.cs#L26 "string DefaultCosmosContainerNameProvider.GetContainerName<TItem>()") | 65 | 4 :heavy_check_mark: | 0 | 8 | 20 / 8 |

<a href="#microsoft-azure-cosmosrepository-providers">:top: back to Microsoft.Azure.CosmosRepository.Providers</a>

</details>

<details>
<summary>
  <strong id="defaultcosmoscontainerprovidertitem">
    DefaultCosmosContainerProvider&lt;TItem&gt; :radioactive:
  </strong>
</summary>
<br>

- The `DefaultCosmosContainerProvider<TItem>` contains 8 members.
- 92 total lines of source code.
- Approximately 25 lines of executable code.
- The highest cyclomatic complexity is 10 :radioactive:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [19](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosContainerProvider.cs#L19 "ICosmosClientProvider DefaultCosmosContainerProvider<TItem>._cosmosClientProvider") | 100 | 0 :heavy_check_mark: | 0 | 1 | 1 / 0 |
| Field | [20](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosContainerProvider.cs#L20 "ICosmosItemConfigurationProvider DefaultCosmosContainerProvider<TItem>._cosmosItemConfigurationProvider") | 100 | 0 :heavy_check_mark: | 0 | 1 | 1 / 0 |
| Field | [17](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosContainerProvider.cs#L17 "Lazy<Task<Container>> DefaultCosmosContainerProvider<TItem>._lazyContainer") | 100 | 0 :heavy_check_mark: | 0 | 3 | 1 / 0 |
| Field | [21](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosContainerProvider.cs#L21 "ILogger<DefaultCosmosContainerProvider<TItem>> DefaultCosmosContainerProvider<TItem>._logger") | 100 | 0 :heavy_check_mark: | 0 | 1 | 1 / 0 |
| Field | [18](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosContainerProvider.cs#L18 "RepositoryOptions DefaultCosmosContainerProvider<TItem>._options") | 100 | 0 :heavy_check_mark: | 0 | 1 | 1 / 0 |
| Method | [23](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosContainerProvider.cs#L23 "DefaultCosmosContainerProvider<TItem>.DefaultCosmosContainerProvider(ICosmosClientProvider cosmosClientProvider, IOptions<RepositoryOptions> options, ICosmosItemConfigurationProvider cosmosItemConfigurationProvider, ILogger<DefaultCosmosContainerProvider<TItem>> logger)") | 56 | 10 :radioactive: | 0 | 11 | 39 / 13 |
| Method | [64](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosContainerProvider.cs#L64 "Task<Container> DefaultCosmosContainerProvider<TItem>.GetContainerAsync()") | 100 | 1 :heavy_check_mark: | 0 | 4 | 2 / 1 |
| Method | [66](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosContainerProvider.cs#L66 "Task<Container> DefaultCosmosContainerProvider<TItem>.GetContainerValueFactoryAsync()") | 60 | 3 :heavy_check_mark: | 0 | 12 | 38 / 11 |

<a href="#microsoft-azure-cosmosrepository-providers">:top: back to Microsoft.Azure.CosmosRepository.Providers</a>

</details>

<details>
<summary>
  <strong id="defaultcosmositemconfigurationprovider">
    DefaultCosmosItemConfigurationProvider :heavy_check_mark:
  </strong>
</summary>
<br>

- The `DefaultCosmosItemConfigurationProvider` contains 7 members.
- 30 total lines of source code.
- Approximately 9 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [15](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosItemConfigurationProvider.cs#L15 "ICosmosContainerNameProvider DefaultCosmosItemConfigurationProvider._containerNameProvider") | 100 | 0 :heavy_check_mark: | 0 | 1 | 1 / 0 |
| Field | [16](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosItemConfigurationProvider.cs#L16 "ICosmosPartitionKeyPathProvider DefaultCosmosItemConfigurationProvider._cosmosPartitionKeyPathProvider") | 100 | 0 :heavy_check_mark: | 0 | 1 | 1 / 0 |
| Field | [17](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosItemConfigurationProvider.cs#L17 "ICosmosUniqueKeyPolicyProvider DefaultCosmosItemConfigurationProvider._cosmosUniqueKeyPolicyProvider") | 100 | 0 :heavy_check_mark: | 0 | 1 | 1 / 0 |
| Field | [13](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosItemConfigurationProvider.cs#L13 "ConcurrentDictionary<Type, ItemOptions> DefaultCosmosItemConfigurationProvider._itemOptionsMap") | 93 | 0 :heavy_check_mark: | 0 | 3 | 1 / 1 |
| Method | [19](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosItemConfigurationProvider.cs#L19 "DefaultCosmosItemConfigurationProvider.DefaultCosmosItemConfigurationProvider(ICosmosContainerNameProvider containerNameProvider, ICosmosPartitionKeyPathProvider cosmosPartitionKeyPathProvider, ICosmosUniqueKeyPolicyProvider cosmosUniqueKeyPolicyProvider)") | 79 | 1 :heavy_check_mark: | 0 | 3 | 9 / 3 |
| Method | [32](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosItemConfigurationProvider.cs#L32 "ItemOptions DefaultCosmosItemConfigurationProvider.AddOptions<TItem>(Type optionType)") | 74 | 1 :heavy_check_mark: | 0 | 6 | 8 / 4 |
| Method | [29](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosItemConfigurationProvider.cs#L29 "ItemOptions DefaultCosmosItemConfigurationProvider.GetOptions<TItem>()") | 100 | 1 :heavy_check_mark: | 0 | 5 | 2 / 1 |

<a href="#microsoft-azure-cosmosrepository-providers">:top: back to Microsoft.Azure.CosmosRepository.Providers</a>

</details>

<details>
<summary>
  <strong id="defaultcosmospartitionkeypathprovider">
    DefaultCosmosPartitionKeyPathProvider :heavy_check_mark:
  </strong>
</summary>
<br>

- The `DefaultCosmosPartitionKeyPathProvider` contains 3 members.
- 30 total lines of source code.
- Approximately 8 lines of executable code.
- The highest cyclomatic complexity is 4 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [17](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosPartitionKeyPathProvider.cs#L17 "IOptions<RepositoryOptions> DefaultCosmosPartitionKeyPathProvider._options") | 100 | 0 :heavy_check_mark: | 0 | 2 | 1 / 0 |
| Method | [19](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosPartitionKeyPathProvider.cs#L19 "DefaultCosmosPartitionKeyPathProvider.DefaultCosmosPartitionKeyPathProvider(IOptions<RepositoryOptions> options)") | 92 | 2 :heavy_check_mark: | 0 | 5 | 4 / 1 |
| Method | [25](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosPartitionKeyPathProvider.cs#L25 "string DefaultCosmosPartitionKeyPathProvider.GetPartitionKeyPath<TItem>()") | 66 | 4 :heavy_check_mark: | 0 | 7 | 18 / 7 |

<a href="#microsoft-azure-cosmosrepository-providers">:top: back to Microsoft.Azure.CosmosRepository.Providers</a>

</details>

<details>
<summary>
  <strong id="defaultcosmosuniquekeypolicyprovider">
    DefaultCosmosUniqueKeyPolicyProvider :heavy_check_mark:
  </strong>
</summary>
<br>

- The `DefaultCosmosUniqueKeyPolicyProvider` contains 1 members.
- 43 total lines of source code.
- Approximately 19 lines of executable code.
- The highest cyclomatic complexity is 6 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [17](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/DefaultCosmosUniqueKeyPolicyProvider.cs#L17 "UniqueKeyPolicy DefaultCosmosUniqueKeyPolicyProvider.GetUniqueKeyPolicy<TItem>()") | 55 | 6 :heavy_check_mark: | 0 | 9 | 39 / 19 |

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
  <strong id="icosmoscontainernameprovider">
    ICosmosContainerNameProvider :heavy_check_mark:
  </strong>
</summary>
<br>

- The `ICosmosContainerNameProvider` contains 1 members.
- 15 total lines of source code.
- Approximately 0 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [22](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/ICosmosContainerNameProvider.cs#L22 "string ICosmosContainerNameProvider.GetContainerName<TItem>()") | 100 | 1 :heavy_check_mark: | 0 | 1 | 8 / 0 |

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
  <strong id="icosmositemconfigurationprovider">
    ICosmosItemConfigurationProvider :heavy_check_mark:
  </strong>
</summary>
<br>

- The `ICosmosItemConfigurationProvider` contains 1 members.
- 7 total lines of source code.
- Approximately 0 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [13](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/ICosmosItemConfigurationProvider.cs#L13 "ItemOptions ICosmosItemConfigurationProvider.GetOptions<TItem>()") | 100 | 1 :heavy_check_mark: | 0 | 2 | 1 / 0 |

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

<details>
<summary>
  <strong id="icosmosuniquekeypolicyprovider">
    ICosmosUniqueKeyPolicyProvider :heavy_check_mark:
  </strong>
</summary>
<br>

- The `ICosmosUniqueKeyPolicyProvider` contains 1 members.
- 13 total lines of source code.
- Approximately 0 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [19](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/src/Providers/ICosmosUniqueKeyPolicyProvider.cs#L19 "UniqueKeyPolicy ICosmosUniqueKeyPolicyProvider.GetUniqueKeyPolicy<TItem>()") | 100 | 1 :heavy_check_mark: | 0 | 2 | 6 / 0 |

<a href="#microsoft-azure-cosmosrepository-providers">:top: back to Microsoft.Azure.CosmosRepository.Providers</a>

</details>

</details>

<a href="#microsoft-azure-cosmosrepository">:top: back to Microsoft.Azure.CosmosRepository</a>

<div id='microsoft-azure-cosmosrepositorytests'></div>

## Microsoft.Azure.CosmosRepositoryTests :heavy_check_mark:

The *Microsoft.Azure.CosmosRepositoryTests.csproj* project file contains:

- 4 namespaces.
- 32 named types.
- 953 total lines of source code.
- Approximately 333 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

<details>
<summary>
  <strong id="microsoft-azure-cosmosrepositorytests">
    Microsoft.Azure.CosmosRepositoryTests :heavy_check_mark:
  </strong>
</summary>
<br>

The `Microsoft.Azure.CosmosRepositoryTests` namespace contains 8 named types.

- 8 named types.
- 468 total lines of source code.
- Approximately 168 lines of executable code.
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
| Property | [67](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/DefaultRepositoryFactoryTests.cs#L67 "string CustomEntityBase.PartitionKey") | 100 | 2 :heavy_check_mark: | 0 | 1 | 1 / 2 |
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

<details>
<summary>
  <strong id="dog">
    Dog :heavy_check_mark:
  </strong>
</summary>
<br>

- The `Dog` contains 3 members.
- 11 total lines of source code.
- Approximately 2 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [32](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/InMemoryRepositoryTests.cs#L32 "Dog.Dog(string breed)") | 96 | 1 :heavy_check_mark: | 0 | 0 | 4 / 1 |
| Property | [28](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/InMemoryRepositoryTests.cs#L28 "string Dog.Breed") | 100 | 1 :heavy_check_mark: | 0 | 0 | 1 / 0 |
| Method | [30](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/InMemoryRepositoryTests.cs#L30 "string Dog.GetPartitionKeyValue()") | 100 | 1 :heavy_check_mark: | 0 | 0 | 1 / 1 |

<a href="#microsoft-azure-cosmosrepositorytests">:top: back to Microsoft.Azure.CosmosRepositoryTests</a>

</details>

<details>
<summary>
  <strong id="inmemoryrepositorytests">
    InMemoryRepositoryTests :heavy_check_mark:
  </strong>
</summary>
<br>

- The `InMemoryRepositoryTests` contains 25 members.
- 378 total lines of source code.
- Approximately 139 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [41](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/InMemoryRepositoryTests.cs#L41 "InMemoryRepository<Dog> InMemoryRepositoryTests._dogRepository") | 100 | 0 :heavy_check_mark: | 0 | 2 | 1 / 0 |
| Field | [40](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/InMemoryRepositoryTests.cs#L40 "InMemoryRepository<Person> InMemoryRepositoryTests._personRepository") | 100 | 0 :heavy_check_mark: | 0 | 2 | 1 / 0 |
| Method | [43](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/InMemoryRepositoryTests.cs#L43 "InMemoryRepositoryTests.InMemoryRepositoryTests()") | 88 | 1 :heavy_check_mark: | 0 | 3 | 5 / 2 |
| Method | [178](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/InMemoryRepositoryTests.cs#L178 "Task InMemoryRepositoryTests.CreateAsync_Item_CreatesItem()") | 65 | 1 :heavy_check_mark: | 0 | 5 | 19 / 9 |
| Method | [165](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/InMemoryRepositoryTests.cs#L165 "Task InMemoryRepositoryTests.CreateAsync_ItemWhereIdAlreadyExists_ThrowsCosmosException()") | 72 | 1 :heavy_check_mark: | 0 | 6 | 12 / 5 |
| Method | [198](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/InMemoryRepositoryTests.cs#L198 "Task InMemoryRepositoryTests.CreateAsync_ManyItems_CreatesAllItems()") | 59 | 2 :heavy_check_mark: | 0 | 7 | 28 / 13 |
| Method | [227](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/InMemoryRepositoryTests.cs#L227 "Task InMemoryRepositoryTests.CreateAsync_ManyItemsWhereOneHasIdThatAlreadyExists_CreatesInitalItemsThenThrowsCosmosException()") | 59 | 2 :heavy_check_mark: | 0 | 7 | 28 / 13 |
| Method | [268](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/InMemoryRepositoryTests.cs#L268 "Task InMemoryRepositoryTests.DeleteAsync_IdPartitionKeyThatDoesNotExist_ThrowsCosmosException()") | 82 | 1 :heavy_check_mark: | 0 | 7 | 11 / 3 |
| Method | [257](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/InMemoryRepositoryTests.cs#L257 "Task InMemoryRepositoryTests.DeleteAsync_IdThatDoesNotExist_ThrowsCosmosException()") | 82 | 1 :heavy_check_mark: | 0 | 6 | 10 / 3 |
| Method | [294](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/InMemoryRepositoryTests.cs#L294 "Task InMemoryRepositoryTests.DeleteAsync_ItemWithIdAndPartitionKeyThatExists_DeletesItem()") | 75 | 1 :heavy_check_mark: | 0 | 5 | 13 / 4 |
| Method | [280](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/InMemoryRepositoryTests.cs#L280 "Task InMemoryRepositoryTests.DeleteAsync_ItemWithIdThatExists_DeletesItem()") | 76 | 1 :heavy_check_mark: | 0 | 5 | 13 / 4 |
| Method | [396](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/InMemoryRepositoryTests.cs#L396 "Task InMemoryRepositoryTests.ExistsAsync_CountQueryWithItemsThatMatch_ReturnsTrue()") | 64 | 2 :heavy_check_mark: | 0 | 5 | 18 / 9 |
| Method | [380](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/InMemoryRepositoryTests.cs#L380 "Task InMemoryRepositoryTests.ExistsAsync_PointReadWhenDoesNotItemsExists_ReturnsFalse()") | 75 | 1 :heavy_check_mark: | 0 | 5 | 14 / 4 |
| Method | [351](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/InMemoryRepositoryTests.cs#L351 "Task InMemoryRepositoryTests.ExistsAsync_PointReadWhenItemsExists_ReturnsTrue()") | 76 | 1 :heavy_check_mark: | 0 | 5 | 14 / 4 |
| Method | [366](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/InMemoryRepositoryTests.cs#L366 "Task InMemoryRepositoryTests.ExistsAsync_PointReadWithPartitionKeyItemsExists_ReturnsTrue()") | 75 | 1 :heavy_check_mark: | 0 | 5 | 13 / 4 |
| Method | [117](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/InMemoryRepositoryTests.cs#L117 "Task InMemoryRepositoryTests.GetAsync_IdAndPartitionKeyObjectExists_GetsItem()") | 69 | 1 :heavy_check_mark: | 0 | 6 | 14 / 6 |
| Method | [72](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/InMemoryRepositoryTests.cs#L72 "Task InMemoryRepositoryTests.GetAsync_IdAndPartitionKeyObjectNotFound_ThrowsCosmosException()") | 82 | 1 :heavy_check_mark: | 0 | 7 | 10 / 3 |
| Method | [100](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/InMemoryRepositoryTests.cs#L100 "Task InMemoryRepositoryTests.GetAsync_IdAndPartitionKeyStringExists_GetsItem()") | 69 | 1 :heavy_check_mark: | 0 | 5 | 16 / 6 |
| Method | [61](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/InMemoryRepositoryTests.cs#L61 "Task InMemoryRepositoryTests.GetAsync_IdAndPartitionKeyStringNotFound_ThrowsCosmosException()") | 82 | 1 :heavy_check_mark: | 0 | 6 | 10 / 3 |
| Method | [50](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/InMemoryRepositoryTests.cs#L50 "Task InMemoryRepositoryTests.GetAsync_IdThatDoesNotExist_ThrowsCosmosException()") | 84 | 1 :heavy_check_mark: | 0 | 6 | 10 / 3 |
| Method | [83](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/InMemoryRepositoryTests.cs#L83 "Task InMemoryRepositoryTests.GetAsync_IdThatExists_GetsItem()") | 70 | 1 :heavy_check_mark: | 0 | 5 | 16 / 6 |
| Method | [146](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/InMemoryRepositoryTests.cs#L146 "Task InMemoryRepositoryTests.GetAsync_PredicateThatDoesMatch_ReturnsItemInList()") | 64 | 1 :heavy_check_mark: | 0 | 7 | 18 / 10 |
| Method | [132](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/InMemoryRepositoryTests.cs#L132 "Task InMemoryRepositoryTests.GetAsync_PredicateThatDoesNotMatch_ReturnsEmptyList()") | 72 | 1 :heavy_check_mark: | 0 | 6 | 13 / 5 |
| Method | [308](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/InMemoryRepositoryTests.cs#L308 "Task InMemoryRepositoryTests.UpdateAsync_ItemThatDoesNotExist_AddsItem()") | 65 | 1 :heavy_check_mark: | 0 | 5 | 19 / 9 |
| Method | [328](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/InMemoryRepositoryTests.cs#L328 "Task InMemoryRepositoryTests.UpdateAsync_ItemThatExists_UpdatesItem()") | 62 | 1 :heavy_check_mark: | 0 | 5 | 22 / 11 |

<a href="#microsoft-azure-cosmosrepositorytests">:top: back to Microsoft.Azure.CosmosRepositoryTests</a>

</details>

<details>
<summary>
  <strong id="person">
    Person :heavy_check_mark:
  </strong>
</summary>
<br>

- The `Person` contains 2 members.
- 9 total lines of source code.
- Approximately 1 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [19](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/InMemoryRepositoryTests.cs#L19 "Person.Person(string name)") | 96 | 1 :heavy_check_mark: | 0 | 0 | 4 / 1 |
| Property | [17](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/InMemoryRepositoryTests.cs#L17 "string Person.Name") | 100 | 1 :heavy_check_mark: | 0 | 0 | 1 / 0 |

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
- 40 total lines of source code.
- Approximately 12 lines of executable code.
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

- The `ServiceCollectionExtensionsTests` contains 2 members.
- 12 total lines of source code.
- Approximately 4 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [13](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Extensions/ServiceCollectionExtensionsTests.cs#L13 "void ServiceCollectionExtensionsTests.AddCosmosRepositoryThrowsWithNullServiceCollection()") | 93 | 1 :heavy_check_mark: | 0 | 3 | 4 / 2 |
| Method | [18](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Extensions/ServiceCollectionExtensionsTests.cs#L18 "void ServiceCollectionExtensionsTests.AddInMemoryCosmosRepositoryThrowsWithNullServiceCollection()") | 93 | 1 :heavy_check_mark: | 0 | 3 | 4 / 2 |

<a href="#microsoft-azure-cosmosrepositorytests-extensions">:top: back to Microsoft.Azure.CosmosRepositoryTests.Extensions</a>

</details>

</details>

<details>
<summary>
  <strong id="microsoft-azure-cosmosrepositorytests-options">
    Microsoft.Azure.CosmosRepositoryTests.Options :heavy_check_mark:
  </strong>
</summary>
<br>

The `Microsoft.Azure.CosmosRepositoryTests.Options` namespace contains 2 named types.

- 2 named types.
- 37 total lines of source code.
- Approximately 14 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

<details>
<summary>
  <strong id="product">
    Product :question:
  </strong>
</summary>
<br>

- The `Product` contains 0 members.
- 4 total lines of source code.
- Approximately 0 lines of executable code.
- The highest cyclomatic complexity is 0 :question:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |

<a href="#microsoft-azure-cosmosrepositorytests-options">:top: back to Microsoft.Azure.CosmosRepositoryTests.Options</a>

</details>

<details>
<summary>
  <strong id="repositoryoptionstests">
    RepositoryOptionsTests :heavy_check_mark:
  </strong>
</summary>
<br>

- The `RepositoryOptionsTests` contains 4 members.
- 29 total lines of source code.
- Approximately 14 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [15](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Options/RepositoryOptionsTests.cs#L15 "void RepositoryOptionsTests.RepositoryOptionsBuilderConfiguresItemCorrectly()") | 72 | 1 :heavy_check_mark: | 0 | 3 | 14 / 6 |
| Method | [38](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Options/RepositoryOptionsTests.cs#L38 "void RepositoryOptionsTests.RepositoryOptionsBuilderThrowsArgumentNullExceptionWhenContainerBuilderActionIsNull()") | 93 | 1 :heavy_check_mark: | 0 | 3 | 3 / 2 |
| Method | [30](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Options/RepositoryOptionsTests.cs#L30 "void RepositoryOptionsTests.RepositoryOptionsBuilderThrowsArgumentNullExceptionWhenContainerNameIsNull()") | 87 | 1 :heavy_check_mark: | 0 | 3 | 3 / 3 |
| Method | [34](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Options/RepositoryOptionsTests.cs#L34 "void RepositoryOptionsTests.RepositoryOptionsBuilderThrowsArgumentNullExceptionWhenPartionKeyIsNull()") | 87 | 1 :heavy_check_mark: | 0 | 3 | 3 / 3 |

<a href="#microsoft-azure-cosmosrepositorytests-options">:top: back to Microsoft.Azure.CosmosRepositoryTests.Options</a>

</details>

</details>

<details>
<summary>
  <strong id="microsoft-azure-cosmosrepositorytests-providers">
    Microsoft.Azure.CosmosRepositoryTests.Providers :heavy_check_mark:
  </strong>
</summary>
<br>

The `Microsoft.Azure.CosmosRepositoryTests.Providers` namespace contains 20 named types.

- 20 named types.
- 408 total lines of source code.
- Approximately 139 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

<details>
<summary>
  <strong id="anotherperson">
    AnotherPerson :question:
  </strong>
</summary>
<br>

- The `AnotherPerson` contains 0 members.
- 5 total lines of source code.
- Approximately 2 lines of executable code.
- The highest cyclomatic complexity is 0 :question:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |

<a href="#microsoft-azure-cosmosrepositorytests-providers">:top: back to Microsoft.Azure.CosmosRepositoryTests.Providers</a>

</details>

<details>
<summary>
  <strong id="customcontainernameitem">
    CustomContainerNameItem :question:
  </strong>
</summary>
<br>

- The `CustomContainerNameItem` contains 0 members.
- 4 total lines of source code.
- Approximately 2 lines of executable code.
- The highest cyclomatic complexity is 0 :question:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |

<a href="#microsoft-azure-cosmosrepositorytests-providers">:top: back to Microsoft.Azure.CosmosRepositoryTests.Providers</a>

</details>

<details>
<summary>
  <strong id="customtypeoverridenbyoptions">
    CustomTypeOverridenByOptions :question:
  </strong>
</summary>
<br>

- The `CustomTypeOverridenByOptions` contains 0 members.
- 4 total lines of source code.
- Approximately 2 lines of executable code.
- The highest cyclomatic complexity is 0 :question:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |

<a href="#microsoft-azure-cosmosrepositorytests-providers">:top: back to Microsoft.Azure.CosmosRepositoryTests.Providers</a>

</details>

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
| Method | [36](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosClientProviderTests.cs#L36 "void DefaultCosmosClientProviderTests.DefaultCosmosClientProviderCorrectlyDisposesOverload()") | 70 | 1 :heavy_check_mark: | 0 | 8 | 19 / 8 |
| Method | [17](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosClientProviderTests.cs#L17 "void DefaultCosmosClientProviderTests.NewDefaultCosmosClientProviderThrowsWithNullCosmosClientOptionsProvider()") | 85 | 1 :heavy_check_mark: | 0 | 5 | 11 / 2 |
| Method | [29](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosClientProviderTests.cs#L29 "void DefaultCosmosClientProviderTests.NewDefaultCosmosClientProviderThrowsWithNullRepositoryOptionsOverload()") | 93 | 1 :heavy_check_mark: | 0 | 5 | 6 / 2 |

<a href="#microsoft-azure-cosmosrepositorytests-providers">:top: back to Microsoft.Azure.CosmosRepositoryTests.Providers</a>

</details>

<details>
<summary>
  <strong id="defaultcosmoscontainernameprovidertests">
    DefaultCosmosContainerNameProviderTests :heavy_check_mark:
  </strong>
</summary>
<br>

- The `DefaultCosmosContainerNameProviderTests` contains 7 members.
- 52 total lines of source code.
- Approximately 20 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [17](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosContainerNameProviderTests.cs#L17 "Mock<IOptions<RepositoryOptions>> DefaultCosmosContainerNameProviderTests._options") | 100 | 0 :heavy_check_mark: | 0 | 3 | 1 / 0 |
| Field | [18](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosContainerNameProviderTests.cs#L18 "RepositoryOptions DefaultCosmosContainerNameProviderTests._repositoryOptions") | 100 | 0 :heavy_check_mark: | 0 | 1 | 1 / 0 |
| Method | [20](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosContainerNameProviderTests.cs#L20 "DefaultCosmosContainerNameProviderTests.DefaultCosmosContainerNameProviderTests()") | 78 | 1 :heavy_check_mark: | 0 | 4 | 6 / 4 |
| Method | [57](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosContainerNameProviderTests.cs#L57 "void DefaultCosmosContainerNameProviderTests.CosmosContainerNameProviderGetsNameForTypeWhenEmptyStringProvidedByOptions()") | 74 | 1 :heavy_check_mark: | 0 | 7 | 10 / 5 |
| Method | [46](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosContainerNameProviderTests.cs#L46 "void DefaultCosmosContainerNameProviderTests.CosmosContainerNameProviderGetsNameForTypeWhenProvidedByOptions()") | 74 | 1 :heavy_check_mark: | 0 | 7 | 10 / 5 |
| Method | [28](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosContainerNameProviderTests.cs#L28 "void DefaultCosmosContainerNameProviderTests.CosmosContainerNameProviderGetsNameFromAttribute()") | 81 | 1 :heavy_check_mark: | 0 | 7 | 8 / 3 |
| Method | [37](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosContainerNameProviderTests.cs#L37 "void DefaultCosmosContainerNameProviderTests.CosmosContainerNameProviderGetsNameFromType()") | 81 | 1 :heavy_check_mark: | 0 | 7 | 8 / 3 |

<a href="#microsoft-azure-cosmosrepositorytests-providers">:top: back to Microsoft.Azure.CosmosRepositoryTests.Providers</a>

</details>

<details>
<summary>
  <strong id="defaultcosmoscontainerprovidertests">
    DefaultCosmosContainerProviderTests :heavy_check_mark:
  </strong>
</summary>
<br>

- The `DefaultCosmosContainerProviderTests` contains 9 members.
- 84 total lines of source code.
- Approximately 14 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [21](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosContainerProviderTests.cs#L21 "Mock<ICosmosItemConfigurationProvider> DefaultCosmosContainerProviderTests._itemConfigurationProvider") | 93 | 0 :heavy_check_mark: | 0 | 2 | 1 / 1 |
| Field | [19](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosContainerProviderTests.cs#L19 "ILoggerFactory DefaultCosmosContainerProviderTests._loggerFactory") | 93 | 0 :heavy_check_mark: | 0 | 2 | 1 / 1 |
| Field | [20](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosContainerProviderTests.cs#L20 "Mock<IOptions<RepositoryOptions>> DefaultCosmosContainerProviderTests._options") | 93 | 0 :heavy_check_mark: | 0 | 3 | 1 / 1 |
| Method | [49](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosContainerProviderTests.cs#L49 "void DefaultCosmosContainerProviderTests.NewDefaultCosmosContainerProviderThrowsWithNullConnectionString()") | 85 | 1 :heavy_check_mark: | 0 | 9 | 12 / 2 |
| Method | [74](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosContainerProviderTests.cs#L74 "void DefaultCosmosContainerProviderTests.NewDefaultCosmosContainerProviderThrowsWithNullContainerId()") | 85 | 1 :heavy_check_mark: | 0 | 9 | 13 / 2 |
| Method | [24](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosContainerProviderTests.cs#L24 "void DefaultCosmosContainerProviderTests.NewDefaultCosmosContainerProviderThrowsWithNullCosmosClient()") | 83 | 1 :heavy_check_mark: | 0 | 9 | 13 / 2 |
| Method | [61](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosContainerProviderTests.cs#L61 "void DefaultCosmosContainerProviderTests.NewDefaultCosmosContainerProviderThrowsWithNullDatabaseId()") | 85 | 1 :heavy_check_mark: | 0 | 9 | 12 / 2 |
| Method | [88](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosContainerProviderTests.cs#L88 "void DefaultCosmosContainerProviderTests.NewDefaultCosmosContainerProviderThrowsWithNullLogger()") | 84 | 1 :heavy_check_mark: | 0 | 9 | 13 / 2 |
| Method | [40](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosContainerProviderTests.cs#L40 "void DefaultCosmosContainerProviderTests.NewDefaultCosmosContainerProviderThrowsWithNullOptions()") | 87 | 1 :heavy_check_mark: | 0 | 8 | 8 / 2 |

<a href="#microsoft-azure-cosmosrepositorytests-providers">:top: back to Microsoft.Azure.CosmosRepositoryTests.Providers</a>

</details>

<details>
<summary>
  <strong id="defaultcosmositemconfigurationprovidertests">
    DefaultCosmosItemConfigurationProviderTests :heavy_check_mark:
  </strong>
</summary>
<br>

- The `DefaultCosmosItemConfigurationProviderTests` contains 4 members.
- 32 total lines of source code.
- Approximately 14 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [15](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosItemConfigurationProviderTests.cs#L15 "Mock<ICosmosContainerNameProvider> DefaultCosmosItemConfigurationProviderTests._containerNameProvider") | 93 | 0 :heavy_check_mark: | 0 | 2 | 1 / 1 |
| Field | [16](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosItemConfigurationProviderTests.cs#L16 "Mock<ICosmosPartitionKeyPathProvider> DefaultCosmosItemConfigurationProviderTests._partitionKeyPathProvider") | 93 | 0 :heavy_check_mark: | 0 | 2 | 1 / 1 |
| Field | [17](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosItemConfigurationProviderTests.cs#L17 "Mock<ICosmosUniqueKeyPolicyProvider> DefaultCosmosItemConfigurationProviderTests._uniqueKeyPolicyProvider") | 93 | 0 :heavy_check_mark: | 0 | 2 | 1 / 1 |
| Method | [20](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosItemConfigurationProviderTests.cs#L20 "void DefaultCosmosItemConfigurationProviderTests.GetOptionsAlwaysGetOptionsForItem()") | 62 | 1 :heavy_check_mark: | 0 | 10 | 20 / 12 |

<a href="#microsoft-azure-cosmosrepositorytests-providers">:top: back to Microsoft.Azure.CosmosRepositoryTests.Providers</a>

</details>

<details>
<summary>
  <strong id="defaultcosmospartitionkeypathprovidertests">
    DefaultCosmosPartitionKeyPathProviderTests :heavy_check_mark:
  </strong>
</summary>
<br>

- The `DefaultCosmosPartitionKeyPathProviderTests` contains 6 members.
- 44 total lines of source code.
- Approximately 18 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Field | [17](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosPartitionKeyPathProviderTests.cs#L17 "Mock<IOptions<RepositoryOptions>> DefaultCosmosPartitionKeyPathProviderTests._options") | 100 | 0 :heavy_check_mark: | 0 | 3 | 1 / 0 |
| Field | [18](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosPartitionKeyPathProviderTests.cs#L18 "RepositoryOptions DefaultCosmosPartitionKeyPathProviderTests._repositoryOptions") | 100 | 0 :heavy_check_mark: | 0 | 1 | 1 / 0 |
| Method | [20](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosPartitionKeyPathProviderTests.cs#L20 "DefaultCosmosPartitionKeyPathProviderTests.DefaultCosmosPartitionKeyPathProviderTests()") | 78 | 1 :heavy_check_mark: | 0 | 4 | 6 / 4 |
| Method | [28](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosPartitionKeyPathProviderTests.cs#L28 "void DefaultCosmosPartitionKeyPathProviderTests.CosmosCosmosPartitionKeyPathProviderCorrectlyGetsPathWhenOptionsAreDefined()") | 74 | 1 :heavy_check_mark: | 0 | 7 | 10 / 5 |
| Method | [39](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosPartitionKeyPathProviderTests.cs#L39 "void DefaultCosmosPartitionKeyPathProviderTests.CosmosCosmosPartitionKeyPathProviderCorrectlyGetsPathWhenOptionsAreDefinedButNull()") | 74 | 1 :heavy_check_mark: | 0 | 7 | 10 / 5 |
| Method | [50](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosPartitionKeyPathProviderTests.cs#L50 "void DefaultCosmosPartitionKeyPathProviderTests.CosmosPartitionKeyPathProviderCorrectlyGetsPathWhenAttributeIsDefined()") | 77 | 1 :heavy_check_mark: | 0 | 10 | 9 / 4 |

<a href="#microsoft-azure-cosmosrepositorytests-providers">:top: back to Microsoft.Azure.CosmosRepositoryTests.Providers</a>

</details>

<details>
<summary>
  <strong id="defaultcosmosuniquekeypolicyprovidertests">
    DefaultCosmosUniqueKeyPolicyProviderTests :heavy_check_mark:
  </strong>
</summary>
<br>

- The `DefaultCosmosUniqueKeyPolicyProviderTests` contains 4 members.
- 48 total lines of source code.
- Approximately 22 lines of executable code.
- The highest cyclomatic complexity is 1 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Method | [53](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosUniqueKeyPolicyProviderTests.cs#L53 "void DefaultCosmosUniqueKeyPolicyProviderTests.CosmosUniqueKeyPolicyProviderCorrectlyGetsNullWhenNoAttributesAreApplied()") | 84 | 1 :heavy_check_mark: | 0 | 5 | 8 / 3 |
| Method | [16](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosUniqueKeyPolicyProviderTests.cs#L16 "void DefaultCosmosUniqueKeyPolicyProviderTests.CosmosUniqueKeyPolicyProviderCorrectlyGetsOneUniqueKey()") | 82 | 1 :heavy_check_mark: | 0 | 5 | 8 / 3 |
| Method | [36](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosUniqueKeyPolicyProviderTests.cs#L36 "void DefaultCosmosUniqueKeyPolicyProviderTests.CosmosUniqueKeyPolicyProviderCorrectlyGetsTwoUniqueKeys()") | 63 | 1 :heavy_check_mark: | 0 | 6 | 16 / 11 |
| Method | [25](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosUniqueKeyPolicyProviderTests.cs#L25 "void DefaultCosmosUniqueKeyPolicyProviderTests.CosmosUniqueKeyPolicyProviderCorrectlyGetsUniqueKeysWithTwoPaths()") | 75 | 1 :heavy_check_mark: | 0 | 5 | 10 / 5 |

<a href="#microsoft-azure-cosmosrepositorytests-providers">:top: back to Microsoft.Azure.CosmosRepositoryTests.Providers</a>

</details>

<details>
<summary>
  <strong id="defaultcosmositemconfigurationprovidertests-item1">
    DefaultCosmosItemConfigurationProviderTests.Item1 :question:
  </strong>
</summary>
<br>

- The `DefaultCosmosItemConfigurationProviderTests.Item1` contains 0 members.
- 4 total lines of source code.
- Approximately 0 lines of executable code.
- The highest cyclomatic complexity is 0 :question:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |

<a href="#microsoft-azure-cosmosrepositorytests-providers">:top: back to Microsoft.Azure.CosmosRepositoryTests.Providers</a>

</details>

<details>
<summary>
  <strong id="person">
    Person :question:
  </strong>
</summary>
<br>

- The `Person` contains 0 members.
- 5 total lines of source code.
- Approximately 2 lines of executable code.
- The highest cyclomatic complexity is 0 :question:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |

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
| Method | [78](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosPartitionKeyPathProviderTests.cs#L78 "string PickleChipsItem.GetPartitionKeyValue()") | 100 | 1 :heavy_check_mark: | 0 | 0 | 1 / 1 |
| Property | [76](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosPartitionKeyPathProviderTests.cs#L76 "string PickleChipsItem.PartitionBits") | 100 | 2 :heavy_check_mark: | 0 | 2 | 2 / 3 |

<a href="#microsoft-azure-cosmosrepositorytests-providers">:top: back to Microsoft.Azure.CosmosRepositoryTests.Providers</a>

</details>

<details>
<summary>
  <strong id="someinterestingclass">
    SomeInterestingClass :heavy_check_mark:
  </strong>
</summary>
<br>

- The `SomeInterestingClass` contains 3 members.
- 8 total lines of source code.
- Approximately 5 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Property | [65](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosUniqueKeyPolicyProviderTests.cs#L65 "string SomeInterestingClass.HouseNumber") | 100 | 2 :heavy_check_mark: | 0 | 0 | 1 / 1 |
| Property | [68](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosUniqueKeyPolicyProviderTests.cs#L68 "string SomeInterestingClass.Name") | 100 | 2 :heavy_check_mark: | 0 | 1 | 2 / 3 |
| Property | [64](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosUniqueKeyPolicyProviderTests.cs#L64 "string SomeInterestingClass.Street") | 100 | 2 :heavy_check_mark: | 0 | 0 | 1 / 1 |

<a href="#microsoft-azure-cosmosrepositorytests-providers">:top: back to Microsoft.Azure.CosmosRepositoryTests.Providers</a>

</details>

<details>
<summary>
  <strong id="someinterestingclass2">
    SomeInterestingClass2 :heavy_check_mark:
  </strong>
</summary>
<br>

- The `SomeInterestingClass2` contains 3 members.
- 10 total lines of source code.
- Approximately 7 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Property | [77](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosUniqueKeyPolicyProviderTests.cs#L77 "string SomeInterestingClass2.HouseNumber") | 100 | 2 :heavy_check_mark: | 0 | 1 | 2 / 3 |
| Property | [79](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosUniqueKeyPolicyProviderTests.cs#L79 "string SomeInterestingClass2.Name") | 100 | 2 :heavy_check_mark: | 0 | 0 | 1 / 1 |
| Property | [74](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosUniqueKeyPolicyProviderTests.cs#L74 "string SomeInterestingClass2.Street") | 100 | 2 :heavy_check_mark: | 0 | 1 | 2 / 3 |

<a href="#microsoft-azure-cosmosrepositorytests-providers">:top: back to Microsoft.Azure.CosmosRepositoryTests.Providers</a>

</details>

<details>
<summary>
  <strong id="someinterestingclass3">
    SomeInterestingClass3 :heavy_check_mark:
  </strong>
</summary>
<br>

- The `SomeInterestingClass3` contains 3 members.
- 11 total lines of source code.
- Approximately 9 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Property | [88](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosUniqueKeyPolicyProviderTests.cs#L88 "string SomeInterestingClass3.HouseNumber") | 100 | 2 :heavy_check_mark: | 0 | 1 | 2 / 3 |
| Property | [91](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosUniqueKeyPolicyProviderTests.cs#L91 "string SomeInterestingClass3.Name") | 100 | 2 :heavy_check_mark: | 0 | 1 | 2 / 3 |
| Property | [85](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosUniqueKeyPolicyProviderTests.cs#L85 "string SomeInterestingClass3.Street") | 100 | 2 :heavy_check_mark: | 0 | 1 | 2 / 3 |

<a href="#microsoft-azure-cosmosrepositorytests-providers">:top: back to Microsoft.Azure.CosmosRepositoryTests.Providers</a>

</details>

<details>
<summary>
  <strong id="someinterestingclass4">
    SomeInterestingClass4 :heavy_check_mark:
  </strong>
</summary>
<br>

- The `SomeInterestingClass4` contains 3 members.
- 8 total lines of source code.
- Approximately 3 lines of executable code.
- The highest cyclomatic complexity is 2 :heavy_check_mark:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |
| Property | [98](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosUniqueKeyPolicyProviderTests.cs#L98 "string SomeInterestingClass4.HouseNumber") | 100 | 2 :heavy_check_mark: | 0 | 0 | 1 / 1 |
| Property | [100](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosUniqueKeyPolicyProviderTests.cs#L100 "string SomeInterestingClass4.Name") | 100 | 2 :heavy_check_mark: | 0 | 0 | 1 / 1 |
| Property | [96](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosUniqueKeyPolicyProviderTests.cs#L96 "string SomeInterestingClass4.Street") | 100 | 2 :heavy_check_mark: | 0 | 0 | 1 / 1 |

<a href="#microsoft-azure-cosmosrepositorytests-providers">:top: back to Microsoft.Azure.CosmosRepositoryTests.Providers</a>

</details>

<details>
<summary>
  <strong id="someotheritem">
    SomeOtherItem :question:
  </strong>
</summary>
<br>

- The `SomeOtherItem` contains 0 members.
- 4 total lines of source code.
- Approximately 0 lines of executable code.
- The highest cyclomatic complexity is 0 :question:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |

<a href="#microsoft-azure-cosmosrepositorytests-providers">:top: back to Microsoft.Azure.CosmosRepositoryTests.Providers</a>

</details>

<details>
<summary>
  <strong id="somethingitem">
    SomethingItem :question:
  </strong>
</summary>
<br>

- The `SomethingItem` contains 0 members.
- 3 total lines of source code.
- Approximately 0 lines of executable code.
- The highest cyclomatic complexity is 0 :question:.

| Member kind | Line number | Maintainability index | Cyclomatic complexity | Depth of inheritance | Class coupling | Lines of source / executable code |
| :-: | :-: | :-: | :-: | :-: | :-: | :-: |

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
| Method | [106](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/Microsoft.Azure.CosmosRepository/test/Providers/DefaultCosmosContainerProviderTests.cs#L106 "Task<T> TestCosmosClientProvider.UseClientAsync<T>(Func<CosmosClient, Task<T>> consume)") | 100 | 1 :heavy_check_mark: | 0 | 4 | 2 / 1 |

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
