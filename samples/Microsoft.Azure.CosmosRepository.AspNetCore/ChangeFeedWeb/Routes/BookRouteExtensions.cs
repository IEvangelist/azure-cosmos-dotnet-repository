// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Net;
using ChangedFeedSamples.Shared.Items;
using ChangeFeedWeb.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository;

namespace ChangeFeedWeb.Routes;

public static class BookRouteExtensions
{
    public static IEndpointRouteBuilder MapBookRoutes(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/api/books", CreateBook);
        routes.MapGet("/api/books", GetBooksForAuthor);
        routes.MapGet("/api/books/{id}", GetBookById);
        return routes;
    }

    private static async Task<BookDto> CreateBook(CreateBookDto createBookDto,
        IRepository<Book> bookRepository)
    {
        var book = createBookDto.ToBook();
        Book? createdBook = await bookRepository.CreateAsync(book);

        return createdBook.ToBookDto();
    }

    private static async Task<IEnumerable<BookDto>> GetBooksForAuthor(HttpContext context,
        IRepository<Book> bookRepository,
        [FromQuery] string category)
    {
        IEnumerable<Book> books = await bookRepository.GetAsync(x => x.PartitionKey == category);

        return books.Select(x => x.ToBookDto());
    }

    private static async Task<IResult> GetBookById(string id,
        IRepository<Book> bookRepository,
        IRepository<BookByIdReference> bookByIdReferenceRepository)
    {
        string? category;

        try
        {
            BookByIdReference? idReference = await bookByIdReferenceRepository.GetAsync(id);
            category = idReference.Category;
        }
        catch (CosmosException e) when (e.StatusCode is HttpStatusCode.NotFound)
        {
            Console.WriteLine(e);
            throw;
        }

        try
        {
            Book? book = await bookRepository.GetAsync(id, category);
            return Results.Ok(book.ToBookDto());
        }
        catch (CosmosException e) when (e.StatusCode is HttpStatusCode.NotFound)
        {
            return Results.NotFound();
        }
    }
}