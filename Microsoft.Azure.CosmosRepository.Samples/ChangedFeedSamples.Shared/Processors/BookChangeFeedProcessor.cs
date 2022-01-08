// Copyright (c) IEvangelist. All rights reserved.
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

    public BookChangeFeedProcessor(ILogger<BookChangeFeedProcessor> logger, IRepository<BookByIdReference> bookByIdReferenceRepository)
    {
        _logger = logger;
        _bookByIdReferenceRepository = bookByIdReferenceRepository;
    }

    public async ValueTask HandleAsync(Book book, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Change detected for book with ID: {book.Id}");

        if (!book.HasBeenUpdated)
        {
            await _bookByIdReferenceRepository.CreateAsync(new BookByIdReference(book.Id, book.Category),
                cancellationToken);
        }

        _logger.LogInformation($"Processed change for book with ID: {book.Id}");

        Console.WriteLine($"Processed Change for book with id {book.Id}");
    }
}