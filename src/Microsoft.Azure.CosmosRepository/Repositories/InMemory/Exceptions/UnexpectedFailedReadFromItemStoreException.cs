// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.InMemory.Exceptions
{
    internal sealed class UnexpectedFailedReadFromItemStoreException : InMemoryCosmosException
    {
        internal UnexpectedFailedReadFromItemStoreException() : base("Unexpectedly failed to read from the item store")
        {

        }
    }
}