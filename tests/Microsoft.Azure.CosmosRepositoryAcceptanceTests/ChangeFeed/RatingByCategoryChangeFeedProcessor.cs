// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryAcceptanceTests.ChangeFeed;

public class RatingByCategoryChangeFeedProcessor(
    ILogger<RatingByCategoryChangeFeedProcessor> logger,
    IRepository<RatingByCategory> ratingByCategoryRepository) : IItemChangeFeedProcessor<Rating>
{
    private readonly ILogger<RatingByCategoryChangeFeedProcessor> _logger = logger;
    private readonly IRepository<RatingByCategory> _ratingByCategoryRepository = ratingByCategoryRepository;

    public async ValueTask HandleAsync(Rating rating, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing change for rating");

        RatingByCategory ratingByCategory = new(
            rating.ProductId,
            rating.Stars,
            rating.Text,
            rating.CategoryId);

        await _ratingByCategoryRepository.UpdateAsync(ratingByCategory, cancellationToken: cancellationToken);

        _logger.LogInformation("Processed change for rating");
    }
}