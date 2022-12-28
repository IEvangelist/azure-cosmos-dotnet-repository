// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using ChangedFeedSamples.Shared.Items;

namespace ChangeFeedWeb.Dtos;

public static class BookMappingExtensions
{
    public static Book ToBook(this CreateBookDto createBookDto) =>
        new(createBookDto.Name, createBookDto.Author, createBookDto.Category);

    public static BookDto ToBookDto(this Book book) =>
        new(book.Id, book.Name, book.Author, book.Category);
}