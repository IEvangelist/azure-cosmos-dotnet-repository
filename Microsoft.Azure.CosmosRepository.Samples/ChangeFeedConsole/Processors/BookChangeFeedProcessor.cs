// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.ChangeFeed;

namespace ChangeFeedConsole.Processors;

public class BookChangeFeedProcessor : IItemChangeFeedProcessor<Book>
{
    private readonly IRepository<BookByIdReference> _bookByIdReferenceRepository;

    public BookChangeFeedProcessor(IRepository<BookByIdReference> bookByIdReferenceRepository)
    {
        _bookByIdReferenceRepository = bookByIdReferenceRepository;
    }

    public async ValueTask HandleAsync(Book book, CancellationToken cancellationToken)
    {
        if (!book.HasBeenUpdated)
        {
            await _bookByIdReferenceRepository.CreateAsync(new BookByIdReference(book.Id, book.Category), cancellationToken);
        }
    }
}