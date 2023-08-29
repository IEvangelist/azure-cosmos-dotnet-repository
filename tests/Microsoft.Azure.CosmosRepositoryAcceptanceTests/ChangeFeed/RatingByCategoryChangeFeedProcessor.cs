// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryAcceptanceTests.ChangeFeed;

public class RatingByCategoryChangeFeedProcessor : IItemChangeFeedProcessor<Rating>
{
    private readonly ILogger<RatingByCategoryChangeFeedProcessor> _logger;
    private readonly IRepository<RatingByCategory> _ratingByCategoryRepository;

    public RatingByCategoryChangeFeedProcessor(
        ILogger<RatingByCategoryChangeFeedProcessor> logger,
        IRepository<RatingByCategory> ratingByCategoryRepository)
    {
        _logger = logger;
        _ratingByCategoryRepository = ratingByCategoryRepository;
    }

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