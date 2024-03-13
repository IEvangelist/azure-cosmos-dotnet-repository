// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Logging;

internal static class LoggerExtensions
{
    //Debug Logger Extensions
    public static void LogItemRead<TItem>(
        this ILogger logger,
        TItem item) where TItem : IItem =>
        LoggerMessageDefinitions.ItemRead(logger, JsonConvert.SerializeObject(item), null!);


    //Info Logger Extensions
    public static void LogPointReadStarted<TItem>(
        this ILogger logger,
        string id,
        string partitionKey) where TItem : IItem =>
        LoggerMessageDefinitions.PointReadStarted(logger, typeof(TItem).Name, id, partitionKey, null!);

    //Info Logger Extensions
    public static void LogPointReadStarted<TItem>(
        this ILogger logger,
        string id,
        IEnumerable<string> partitionKeys) where TItem : IItem =>
        LoggerMessageDefinitions.HierarchicalPointReadStarted(logger, typeof(TItem).Name, id, partitionKeys, null!);

    public static void LogPointReadExecuted<TItem>(
        this ILogger logger,
        double ruCharge) where TItem : IItem =>
        LoggerMessageDefinitions.PointReadExecuted(logger, typeof(TItem).Name, ruCharge, null!);

    public static void LogQueryConstructed<TItem>(
        this ILogger logger,
        IQueryable<TItem> queryable) where TItem : IItem =>
        LoggerMessageDefinitions.QueryConstructed(logger, typeof(TItem).Name, queryable.ToString(), null!);

    public static void LogQueryExecuted<TItem>(
        this ILogger logger,
        IQueryable<TItem> queryable,
        double charge) where TItem : IItem =>
        LoggerMessageDefinitions.QueryExecuted(logger, typeof(TItem).Name, charge, queryable.ToString(), null!);

    public static void LogItemNotFoundHandled<TItem>(
        this ILogger logger,
        string id,
        string partitionKey,
        CosmosException e) where TItem : IItem =>
        LoggerMessageDefinitions.ItemNotFoundHandled(logger, typeof(TItem).Name, id, partitionKey, e);

    public static void LogItemNotFoundHandled<TItem>(
        this ILogger logger,
        string id,
        IEnumerable<string> partitionKeys,
        CosmosException e) where TItem : IItem =>
        LoggerMessageDefinitions.HierarchicalItemNotFoundHandled(logger, typeof(TItem).Name, id, partitionKeys, e);
}