// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Linq.Expressions;
using Aspire.Microsoft.Azure.CosmosRepository.Items.Configuration;

namespace AspireSample.ApiService.Infrastructure;

public class EmployeeItemConfiguration : ICosmosItemConfiguration<EmployeeItem>
{
    public string ContainerId { get; } = "employees";
    public string PartitionKeyPath { get; } = "/storeNumber";

    public Expression<Func<EmployeeItem, bool>> LogicalPartitionQuery(
        string partitionKey) =>
        employee => employee.StoreNumber == partitionKey;
}