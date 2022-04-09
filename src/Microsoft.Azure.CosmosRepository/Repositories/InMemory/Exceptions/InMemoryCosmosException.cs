// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.Azure.CosmosRepository.InMemory.Exceptions
{
    internal abstract class InMemoryCosmosException : Exception
    {
        protected InMemoryCosmosException()
        {

        }

        protected InMemoryCosmosException(string message) : base(message)
        {

        }

        protected InMemoryCosmosException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}