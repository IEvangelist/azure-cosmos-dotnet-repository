// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace AspireApp.ApiService;

public static class TodoEndpointExtensions
{
    public static IEndpointRouteBuilder MapTodoApi(this IEndpointRouteBuilder app)
    {
        var todos = app.MapGroup("/api/todos");

        todos.MapGet("", GetTodosAsync)
            .WithOpenApi()
            .Produces<Todo[]>(200)
            .WithHttpLogging(HttpLoggingFields.All);

        todos.MapGet("/{id}", GetTodoAsync)
            .WithOpenApi()
            .Produces<Todo>(200)
            .Produces(404)
            .WithHttpLogging(HttpLoggingFields.All);

        todos.MapPost("", CreateTodoAsync)
            .WithOpenApi()
            .Produces(201)
            .Produces(400)
            .WithHttpLogging(HttpLoggingFields.All);

        todos.MapPut("", UpdateTodoAsync)
            .WithOpenApi()
            .Produces(204)
            .Produces(404)
            .WithHttpLogging(HttpLoggingFields.All);

        todos.MapDelete("/{id}", DeleteTodoAsync)
            .WithOpenApi()
            .Produces(204)
            .Produces(404)
            .WithHttpLogging(HttpLoggingFields.All);

        return app;
    }

    static IAsyncEnumerable<Todo> GetTodosAsync([FromServices] IReadOnlyRepository<Todo> repository)
        => repository.PageAsync(predicate: todo => todo != null, limit: 1_000, pageSize: 25);

    static async Task<IResult> GetTodoAsync([FromRoute] string id, [FromServices] IRepository<Todo> repository)
    {
        var todo = await repository.GetAsync(id);

        return todo is null
            ? Results.NotFound()
            : Results.Json(todo);
    }

    static async Task<IResult> CreateTodoAsync([FromBody] Todo todo, [FromServices] IRepository<Todo> repository)
    {
        var result = await repository.CreateAsync(todo);

        return result is null
            ? Results.BadRequest()
            : Results.Created($"/api/todos/{result.Id}", result);
    }

    static async Task<IResult> UpdateTodoAsync([FromBody] Todo inputTodo, [FromServices] IRepository<Todo> repository)
    {
        await repository.UpdateAsync(inputTodo);

        return Results.NoContent();
    }

    static async Task<IResult> DeleteTodoAsync([FromRoute] string id, [FromServices] IRepository<Todo> repository)
    {
        await repository.DeleteAsync(id);

        return Results.NoContent();
    }
}
