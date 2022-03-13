# Overview

This set of libraries helps implementing an event sourcing architecture with Cosmos DB. It aims to lower the entry barrier and the improve time to market of an application that would benefit from an ES architecture.

:::tip Credits
This library is built on top of the popular Cosmos DB repository pattern implementation [`IEvangelist.Azure.CosmosRepository`](https://ievangelist.github.io/azure-cosmos-dotnet-repository/) package.
:::

## Motivation

Why did we create this project

## Getting Setup

The easiest way to get started is with a simple .NET 6 web application to create one run the command below.

```bash
$ dotnet new web -n MyEventSourcingApplication
```

The next step is install the `IEvangelist.Azure.CosmosEventSourcing` nuget package. This can be done with the command below. This will give you a default web application.

```bash
$ cd MyEventSourcingApplication
$ dotnet add package IEvangelist.Azure.CosmosEventSourcing
```




