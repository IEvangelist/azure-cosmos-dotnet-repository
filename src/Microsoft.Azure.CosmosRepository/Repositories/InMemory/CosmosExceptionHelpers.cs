// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Net;
using Microsoft.Azure.Cosmos;

namespace Microsoft.Azure.CosmosRepository.InMemory
{
    internal static class CosmosExceptionHelpers
    {
        public static CosmosException NotFound() =>
            WithStatusCode(HttpStatusCode.NotFound);

        public static CosmosException Conflict() =>
            WithStatusCode(HttpStatusCode.Conflict);

        public static CosmosException MismatchedEtags() =>
            WithStatusCode(HttpStatusCode.PreconditionFailed);

        private static CosmosException WithStatusCode(HttpStatusCode httpStatusCode) =>
            new (
                string.Empty,
                httpStatusCode,
                0,
                string.Empty,
                0);
    }
}