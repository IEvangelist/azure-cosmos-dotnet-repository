---
title: "Logging"
weight: 1
---

The Azure Cosmos Repository follows the [high performance logging guidelines](https://docs.microsoft.com/aspnet/core/fundamentals/logging/loggermessage?view=aspnetcore-6.0) making use of `LoggerMessage` throughout the library.

This page details what logs we offer and also what `EventId` ranges we have chosen to use.

## Event ID Ranges

Since this library is making use of `EventId`'s each message we log falls within a range. We have split this range based on the common logging levels you would be use to seeing in any normal applications using `ILogger`. See the ranges below.

| Log Level     | `EventId` range |
|---------------|-----------------|
| Debug/Verbose | 15000 - 15100   |
| Information   | 15101 - 15200   |
| Critical      | 15401 - 15500   |
| Warning       | 15201 - 15300   |
| Error         | 15301 - 15400   |

> You can also consult the code for more information [`EventId`s definitions](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/src/Microsoft.Azure.CosmosRepository/Logging/EventIds.cs)