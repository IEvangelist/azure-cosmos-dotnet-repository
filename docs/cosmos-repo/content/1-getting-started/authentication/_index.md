---
title: "Authentication"
weight: 2
---

## Authenticating using an identity

The Azure Cosmos DB .NET SDK also supports authentication using identities, which are considered superior from an audit and granularity of permissions perspective. Authenticating using a connection string essentially provides full access to perform operations within the [data plane](https://docs.microsoft.com/en-us/azure/cosmos-db/role-based-access-control)
of your Cosmos DB Account. More information on the Azure control plane and data plane is available [here](https://docs.microsoft.com/en-us/azure/azure-resource-manager/management/control-plane-and-data-plane).

This library also supports authentication using an identity. To authenticate using an identity (User, Group, Application Registration, or Managed Identity) you will need to set the `AccountEndpoint` and `TokenCredential` options that are available on the `RepositoryOptions` class.

In a basic scenario, there are three steps that need to be completed:

1. If the identity that you would like to use, does not exist in Azure Active Directory, create it now.

1. Use the Azure CLI to [assign](https://docs.microsoft.com/en-us/cli/azure/cosmosdb/sql/role/assignment?view=azure-cli-latest#az_cosmosdb_sql_role_assignment_create) the appropriate role to your identity at the desired scope. - In most cases, using the [built-in roles](https://docs.microsoft.com/en-us/azure/cosmos-db/how-to-setup-rbac#built-in-role-definitions) will be sufficient. However, there is support for creating custom role definitions using the Azure CLI, you can read more on this [here](https://docs.microsoft.com/en-us/azure/cosmos-db/how-to-setup-rbac#role-definitions).
1. Configure your application using the `AddCosmosRepository` method in your `Startup.cs` file:

   ```csharp
   using Azure.Identity;

   public void ConfigureServices(IServiceCollection services)
   {
       DefaultAzureCredential credential = new();

       services.AddCosmosRepository(
           options =>
           {
               options.TokenCredential = credential;
               options.AccountEndpoint = "< account endpoint URI >";
               options.ContainerId = "data-store";
               options.DatabaseId = "samples";
           });
   }
   ```

The example above is using the `DefaultAzureCredential` object provided by the [Azure Identity](https://www.nuget.org/packages/Azure.Identity) NuGet package, which provides seamless integration with Azure Active Directory. More information on this package is available [here](https://docs.microsoft.com/en-us/dotnet/api/overview/azure/identity-readme).