---
title: Overview
lang: en-GB
description: This set of libraries helps implementing an event sourcing architecture with Cosmos DB. It aims to lower the entry barrier and to improve time to market of an application that would benefit from an ES architecture.
---

# Overview

{{ $frontmatter.description }}

:::tip Credits
This library is built on top of the popular Cosmos DB repository pattern implementation [`IEvangelist.Azure.CosmosRepository`](https://ievangelist.github.io/azure-cosmos-dotnet-repository/) package.
:::

## Motivation

The motivation for this project is to lower the entry barrier to building an application that wants to make use of event sourcing. It aims to deal with the technical complexity that comes with this sort of implementation while still allowing flexibility.

Azure's Cosmos DB is a great choice when talking about making it simple to get started with event sourcing, you can have a database up and running in minutes, [see this guide for more information.](https://docs.microsoft.com/en-us/azure/cosmos-db/sql/create-cosmosdb-resources-portal)

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




