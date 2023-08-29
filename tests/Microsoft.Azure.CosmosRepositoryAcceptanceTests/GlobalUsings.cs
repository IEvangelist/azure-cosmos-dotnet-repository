// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Net;
global using System.Threading;
global using System.Threading.Tasks;
global using FluentAssertions;
global using FluentAssertions.Equivalency;
global using Microsoft.Azure.Cosmos;
global using Microsoft.Azure.CosmosRepository;
global using Microsoft.Azure.CosmosRepository.AspNetCore.Extensions;
global using Microsoft.Azure.CosmosRepository.ChangeFeed;
global using Microsoft.Azure.CosmosRepository.Exceptions;
global using Microsoft.Azure.CosmosRepository.Extensions;
global using Microsoft.Azure.CosmosRepository.Options;
global using Microsoft.Azure.CosmosRepository.Paging;
global using Microsoft.Azure.CosmosRepository.Providers;
global using Microsoft.Azure.CosmosRepository.Specification;
global using Microsoft.Azure.CosmosRepositoryAcceptanceTests.Models;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Newtonsoft.Json;
global using Polly;
global using Xunit;
global using Xunit.Abstractions;
