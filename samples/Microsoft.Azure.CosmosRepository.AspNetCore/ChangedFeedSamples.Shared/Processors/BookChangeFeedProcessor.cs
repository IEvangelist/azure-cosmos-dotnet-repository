// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using ChangedFeedSamples.Shared.Items;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.ChangeFeed;
using Microsoft.Extensions.Logging;

namespace ChangedFeedSamples.Shared.Processors;

public class BookChangeFeedProcessor : IItemChangeFeedProcessor<Book>
{
    private readonly ILogger<BookChangeFeedProcessor> _logger;
    private readonly IRepository<BookByIdReference> _bookByIdReferenceRepository;

    public BookChangeFeedProcessor(ILogger<BookChangeFeedProcessor> logger,
        IRepository<BookByIdReference> bookByIdReferenceRepository)
    {
        _logger = logger;
        _bookByIdReferenceRepository = bookByIdReferenceRepository;
    }

    public async ValueTask HandleAsync(Book rating, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Change detected for book with ID: {BookId}", rating.Id);

        if (!rating.HasBeenUpdated)
        {
            await _bookByIdReferenceRepository
                .CreateAsync(new BookByIdReference(rating.Id, rating.Category),
                cancellationToken);
        }

        _logger.LogInformation("Processed change for book with ID: {BookId}", rating.Id);
    }
}