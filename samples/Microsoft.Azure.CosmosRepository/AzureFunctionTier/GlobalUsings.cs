// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

global using AzureFunctionTier.Model;

global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;

global using Microsoft.Azure.CosmosRepository;
global using Microsoft.Azure.CosmosRepository.Paging;
global using Microsoft.Azure.Functions.Extensions.DependencyInjection;
global using Microsoft.Azure.WebJobs;
global using Microsoft.Azure.WebJobs.Extensions.Http;

global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;

global using Newtonsoft.Json;

global using User = AzureFunctionTier.Model.User;
