// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Logging;

internal static class EventIds
{
    //15_000 - 15_100 Debug Events
    public static readonly EventId CosmosItemRead = new(
        15_001,
        nameof(CosmosItemRead));

    //15_101 - 15_200 Info Events
    public static readonly EventId CosmosPointReadStarted = new(
        15_101,
        nameof(CosmosPointReadStarted));

    public static readonly EventId CosmosPointReadExecuted = new(
        15_102,
        nameof(CosmosPointReadExecuted));

    public static readonly EventId CosmosQueryConstructed = new(
        15_103,
        nameof(CosmosQueryConstructed));

    public static readonly EventId CosmosQueryExecuted = new(
        15_104,
        nameof(CosmosQueryExecuted));

    public static readonly EventId ItemNotFoundHandled = new(
        15_105,
        nameof(ItemNotFoundHandled));
}