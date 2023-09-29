// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using ChangedFeedSamples.Shared.Items;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.ChangeFeed;
using Microsoft.Extensions.Logging;

namespace ChangedFeedSamples.Shared.Processors;

public class BookChangeFeedProcessor(ILogger<BookChangeFeedProcessor> logger,
    IRepository<BookByIdReference> bookByIdReferenceRepository) : IItemChangeFeedProcessor<Book>
{
    public async ValueTask HandleAsync(Book rating, CancellationToken cancellationToken)
    {
        logger.LogInformation("Change detected for book with ID: {BookId}", rating.Id);

        if (!rating.HasBeenUpdated)
        {
            await bookByIdReferenceRepository
                .CreateAsync(new BookByIdReference(rating.Id, rating.Category),
                cancellationToken);
        }

        logger.LogInformation("Processed change for book with ID: {BookId}", rating.Id);
    }
}