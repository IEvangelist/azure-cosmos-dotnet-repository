// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Logging;

internal static class LoggerMessageDefinitions
{
    //Debug Definitions
    internal static readonly Action<ILogger, string, Exception?> ItemRead = LoggerMessage.Define<string>(
        LogLevel.Debug,
        EventIds.CosmosItemRead,
        "Cosmos item read {CosmosItemJson}"
    );

    // Info Definitions
    internal static readonly Action<ILogger, string, string, string, Exception?> PointReadStarted =
        LoggerMessage.Define<string, string, string>(
            LogLevel.Information,
            EventIds.CosmosPointReadStarted,
            "Point read started for item type {CosmosItemType} with id {CosmosItemId} and partitionKey {CosmosItemPartitionKey}"
        );

    // Info Definitions
    internal static readonly Action<ILogger, string, string, IEnumerable<string>, Exception?> HierarchicalPointReadStarted =
        LoggerMessage.Define<string, string, IEnumerable<string>>(
            LogLevel.Information,
            EventIds.CosmosPointReadStarted,
            "Point read started for item type {CosmosItemType} with id {CosmosItemId} and partitionKeys {CosmosItemPartitionKeys}"
        );

    internal static readonly Action<ILogger, string, double, Exception?> PointReadExecuted =
        LoggerMessage.Define<string, double>(
            LogLevel.Information,
            EventIds.CosmosPointReadExecuted,
            "Point read executed for item type {CosmosItemType} total RU cost {CosmosOperationRUCharge}"
        );

    internal static readonly Action<ILogger, string, string?, Exception?> QueryConstructed =
        LoggerMessage.Define<string, string?>(
            LogLevel.Information,
            EventIds.CosmosQueryConstructed,
            "Cosmos query constructed for item type {CosmosItemType}: {CosmosQuery}"
        );

    internal static readonly Action<ILogger, string, double, string?, Exception?> QueryExecuted =
        LoggerMessage.Define<string, double, string?>(
            LogLevel.Information,
            EventIds.CosmosQueryExecuted,
            "Cosmos query executed for item type {CosmosItemType} with a charge of {CosmosOperationRUCharge} RUs Query: {CosmosQuery}");

    internal static readonly Action<ILogger, string, string, string, Exception?> ItemNotFoundHandled =
        LoggerMessage.Define<string, string, string>(
            LogLevel.Information,
            EventIds.ItemNotFoundHandled,
            "CosmosException Status Code 404 handled for item of type {CosmosItemType} with {CosmosItemId} and partition key {CosmosItemPartitionKey}"
        );

    internal static readonly Action<ILogger, string, string, IEnumerable<string>, Exception?> HierarchicalItemNotFoundHandled =
        LoggerMessage.Define<string, string, IEnumerable<string>>(
            LogLevel.Information,
            EventIds.ItemNotFoundHandled,
            "CosmosException Status Code 404 handled for item of type {CosmosItemType} with {CosmosItemId} and partition keys {CosmosItemPartitionKeys}"
        );
}