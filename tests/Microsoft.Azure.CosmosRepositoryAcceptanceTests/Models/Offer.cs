// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryAcceptanceTests.Models;

public class Offer : FullItem
{
    public string OfferType { get; }

    public string CreatedBy { get; }

    public string PartitionKey { get; }

    protected override string GetPartitionKeyValue() =>
        PartitionKey;

    public Offer(string offerType, string createdBy)
    {
        OfferType = offerType;
        CreatedBy = createdBy;
        PartitionKey = offerType;
    }
}

public class BuyOneGetOneFreeOffer : Offer
{
    public BuyOneGetOneFreeOffer(string createdBy) : base(nameof(BuyOneGetOneFreeOffer), createdBy)
    {
    }
}

public class DiscountOf20Percent : Offer
{
    public DiscountOf20Percent(string createdBy) : base(nameof(DiscountOf20Percent), createdBy)
    {
    }
}