// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;

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

public class BuyOneGetOneHalfPriceOffer : Offer
{
    public BuyOneGetOneHalfPriceOffer(string createdBy) : base(nameof(BuyOneGetOneHalfPriceOffer), createdBy)
    {
    }
}