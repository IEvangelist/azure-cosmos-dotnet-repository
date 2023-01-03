// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

global using System.Collections.Concurrent;
global using System.Linq.Expressions;
global using System.Net;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using Azure.Core;
global using Microsoft.Azure.Cosmos;
global using Microsoft.Azure.Cosmos.Linq;
global using Microsoft.Azure.CosmosRepository;
global using Microsoft.Azure.CosmosRepository.Attributes;
global using Microsoft.Azure.CosmosRepository.Builders;
global using Microsoft.Azure.CosmosRepository.ChangeFeed;
global using Microsoft.Azure.CosmosRepository.ChangeFeed.InMemory;
global using Microsoft.Azure.CosmosRepository.ChangeFeed.Providers;
global using Microsoft.Azure.CosmosRepository.Exceptions;
global using Microsoft.Azure.CosmosRepository.Extensions;
global using Microsoft.Azure.CosmosRepository.Internals;
global using Microsoft.Azure.CosmosRepository.Logging;
global using Microsoft.Azure.CosmosRepository.Options;
global using Microsoft.Azure.CosmosRepository.Paging;
global using Microsoft.Azure.CosmosRepository.Processors;
global using Microsoft.Azure.CosmosRepository.Providers;
global using Microsoft.Azure.CosmosRepository.Services;
global using Microsoft.Azure.CosmosRepository.Specification;
global using Microsoft.Azure.CosmosRepository.Specification.Builder;
global using Microsoft.Azure.CosmosRepository.Specification.Evaluator;
global using Microsoft.Azure.CosmosRepository.Validators;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Newtonsoft.Json;
global using Newtonsoft.Json.Linq;
global using Newtonsoft.Json.Serialization;
