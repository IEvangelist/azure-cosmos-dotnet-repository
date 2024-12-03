// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace AspireApp.Web.Components.Pages;

public partial class Home(TodoApiClient client)
{
    private List<Todo> _todos = [];

    [SupplyParameterFromForm]
    private Todo Model { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadTodosAsync();
    }

    private async Task LoadTodosAsync()
    {
        _todos = (await client.GetTodosAsync()).Select(Todo.FromDto).ToList();
    }

    private async Task AddTodoAsync()
    {
        if (await client.CreateTodoAsync(Model.ToDto()) is not null)
        {
            Model = new Todo();
            await LoadTodosAsync();
        }
    }

    private async Task OnKeyUp(KeyboardEventArgs args)
    {
        if (args is { Key: "Enter" } && Model is { Title.Length: > 2 })
        {
            await AddTodoAsync();
        }
    }

    private async Task Delete(Todo todo)
    {
        if (await client.DeleteTodoAsync(todo.Id!))
        {
            await LoadTodosAsync();
        }
    }

    private async Task DeleteCompletedAsync()
    {
        var deleted = false;
        foreach (var todo in _todos.Where(t => t.IsCompleted))
        {
            if (await client.DeleteTodoAsync(todo.Id!))
            {
                deleted = true;
            }
        }

        if (deleted)
        {
            await LoadTodosAsync();
        }
    }

    private async Task SaveTodoAsync(Todo todo)
    {
        if (await client.UpdateTodoAsync(todo.ToDto()))
        {
            await LoadTodosAsync();
        }
    }
}

internal class Todo
{
    [Required(ErrorMessage = "Please provide a title.")]
    [MinLength(3, ErrorMessage = "The title must be at least 3 characters long.")]
    public string Title { get; set; } = "";

    public bool IsCompleted { get; set; }

    internal string? Id { get; set; } = Guid.NewGuid().ToString();

    public static Todo FromDto(TodoDto dto) => new()
    {
        Title = dto.Title,
        IsCompleted = dto.IsCompleted,
        Id = dto.Id
    };

    public TodoDto ToDto() => new(Title, IsCompleted, Id);
}
