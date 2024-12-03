// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace AspireApp.ApiService;

/// <summary>
/// Represents a todo item.
/// </summary>
/// <remarks>
/// <example>
/// Consider the following example:
/// <code lang="csharp">
/// Todo todo = "🐕 Walk the dog.";
///
/// // Imagine time passes and you actually walk the dog.
/// // To update the todo, mark it as complete:
///
/// todo.IsCompleted = true;
/// </code>
/// </example>
/// </remarks>
public sealed class Todo : Item
{
    /// <summary>
    /// Gets or sets the title of the todo.
    /// </summary>
    [JsonProperty("title")]
    public string Title { get; set; } = "";

    /// <summary>
    /// Gets or sets whether the todo is considered complete.
    /// </summary>
    [JsonProperty("isCompleted")]
    public bool IsCompleted { get; set; }

    /// <summary>
    /// Implicitly converts the given <see cref="string"/>
    /// <paramref name="title"/> to a <see cref="Todo"/>.
    /// </summary>
    /// <param name="title">The title of the todo.</param>
    /// <returns>
    /// A new <see cref="Todo"/> instance with the
    /// given <paramref name="title"/> as the <see cref="Title"/>.
    /// </returns>
    public static implicit operator Todo(string title) => new()
    {
        Title = title,
        IsCompleted = false
    };
}
