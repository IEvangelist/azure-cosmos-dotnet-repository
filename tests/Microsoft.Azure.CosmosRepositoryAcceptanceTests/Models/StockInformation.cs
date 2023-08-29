// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryAcceptanceTests.Models;

public record StockInformation(int Count, DateTime LastReplenishedUtc, DateTime? DueReplenishmentUtc = null);