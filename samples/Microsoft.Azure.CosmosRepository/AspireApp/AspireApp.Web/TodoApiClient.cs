namespace AspireApp.Web;

public sealed class TodoApiClient(HttpClient httpClient)
{
    private const string BaseRoute = "/api/todos";

    public async Task<TodoDto[]> GetTodosAsync(int maxItems = 10, CancellationToken cancellationToken = default)
    {
        List<TodoDto>? todos = null;

        await foreach (var todo in httpClient.GetFromJsonAsAsyncEnumerable<TodoDto>(BaseRoute, cancellationToken))
        {
            if (todos?.Count >= maxItems)
            {
                break;
            }
            if (todo is not null)
            {
                todos ??= [];
                todos.Add(todo);
            }
        }

        return todos?.ToArray() ?? [];
    }

    public Task<TodoDto?> GetTodoAsync(string id, CancellationToken cancellationToken = default) =>
        httpClient.GetFromJsonAsync<TodoDto>($"{BaseRoute}/{id}", cancellationToken);

    public async Task<TodoDto?> CreateTodoAsync(TodoDto todo, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(BaseRoute, todo, cancellationToken);

        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<TodoDto>(cancellationToken)
            : null;
    }

    public async Task<bool> UpdateTodoAsync(TodoDto todo, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PutAsJsonAsync(BaseRoute, todo, cancellationToken);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteTodoAsync(string id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.DeleteAsync($"{BaseRoute}/{id}", cancellationToken);

        return response.IsSuccessStatusCode;
    }
}

public record class TodoDto(string Title, bool IsCompleted, string? Id = null);
