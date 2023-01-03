// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Internals;

class CosmosClientOptionsManipulator
{
    internal Action<CosmosClientOptions> Configure { get; }

    internal CosmosClientOptionsManipulator(Action<CosmosClientOptions>? configure) =>
        Configure = configure ?? (options => { /* if not provided, act as a pass-thru */ });
}
