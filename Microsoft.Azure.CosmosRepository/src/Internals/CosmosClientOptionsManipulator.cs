// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Azure.Cosmos;

namespace Microsoft.Azure.CosmosRepository.Internals
{
    class CosmosClientOptionsManipulator
    {
        internal Action<CosmosClientOptions> Configure { get; }

        internal CosmosClientOptionsManipulator(Action<CosmosClientOptions> configure) =>
            Configure = configure ?? (options => { /* if not provided, act as a pass-thru */ });
    }
}
