// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryAcceptanceTests.Models;

public class Offer(string offerType, string createdBy) : FullItem
{
    public string OfferType { get; } = offerType;

    public string CreatedBy { get; } = createdBy;

    public string PartitionKey { get; } = offerType;

    protected override string GetPartitionKeyValue() =>
        PartitionKey;
}

public class BuyOneGetOneFreeOffer(string createdBy) : Offer(nameof(BuyOneGetOneFreeOffer), createdBy)
{
}

public class DiscountOf20Percent(string createdBy) : Offer(nameof(DiscountOf20Percent), createdBy)
{
}