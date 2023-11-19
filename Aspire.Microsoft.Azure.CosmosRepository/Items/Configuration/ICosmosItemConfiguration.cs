// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Linq.Expressions;
using Microsoft.Azure.Cosmos;

namespace Aspire.Microsoft.Azure.CosmosRepository.Items.Configuration;

public interface ICosmosItemConfiguration<TItem> where TItem : IItem
{
    string Container { get; }

    string PartitionKeyPath { get; }

    Expression<Func<TItem, PartitionKey>> GetPartitionKeyValue() =>
        i => new PartitionKey(i.Id);
}