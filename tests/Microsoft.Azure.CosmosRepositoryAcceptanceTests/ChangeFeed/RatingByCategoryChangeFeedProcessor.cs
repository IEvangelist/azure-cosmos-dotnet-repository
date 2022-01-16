// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.ChangeFeed;
using Microsoft.Azure.CosmosRepositoryAcceptanceTests.Models;

namespace Microsoft.Azure.CosmosRepositoryAcceptanceTests.ChangeFeed;

public class RatingByCategoryChangeFeedProcessor : IItemChangeFeedProcessor<Rating>
{
    private readonly IRepository<RatingByCategory> _ratingByCategoryRepository;

    public RatingByCategoryChangeFeedProcessor(IRepository<RatingByCategory> ratingByCategoryRepository) =>
        _ratingByCategoryRepository = ratingByCategoryRepository;

    public async ValueTask HandleAsync(Rating rating, CancellationToken cancellationToken)
    {
        RatingByCategory ratingByCategory = new(
            rating.ProductId,
            rating.Stars,
            rating.Text,
            rating.CategoryId);

        await _ratingByCategoryRepository.UpdateAsync(ratingByCategory, cancellationToken);
    }
}