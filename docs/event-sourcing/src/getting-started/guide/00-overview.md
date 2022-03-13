---
title: Guide Overview
lang: en-GB
description: This guide aims to talk your through creating an event sourcing application using this library. It aims to cover at a high level the concepts and required elements of an implementation using some of the components of this library.
---

# Overview

{{ $frontmatter.description }}

## Project Setup

The easiest way to get started is with a simple .NET 6 web application to create one run the command below.

```bash
$ dotnet new web -n EventSourcingCustomerAccount
```

The next step is install the `IEvangelist.Azure.CosmosEventSourcing` nuget package. This can be done with the command below. This will give you a default web application.

```bash
$ cd EventSourcingCustomerAccount
$ dotnet add package IEvangelist.Azure.CosmosEventSourcing
```

:::tip Next Steps
Start with your [Domain Implementation here.](./01-domain-implementation.md)
:::