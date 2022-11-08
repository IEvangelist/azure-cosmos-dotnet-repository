using Newtonsoft.Json;

namespace AzureFunctionTier.Model;

public class PostUserRequest
{
    [JsonProperty("firstName")]
    public required string FirstName { get; set; }

    [JsonProperty("lastName")]
    public required string LastName { get; set; }

    [JsonProperty("emailAddress")]
    public required string EmailAddress { get; set; }

    #region Converters

    /// <summary>
    /// Create a <see cref="User"/> out of a <see cref="PostUserRequest"/>.
    /// </summary>
    /// <param name="postUserRequest">The <see cref="PostUserRequest"/> to start with.</param>
    public static implicit operator User(PostUserRequest postUserRequest) =>
        new()
        {
            FirstName = postUserRequest?.FirstName ?? "Jane",
            LastName = postUserRequest?.LastName ?? "Doe",
            EmailAddress = postUserRequest?.EmailAddress ?? "jane.doe@example.com",
        };

    #endregion
}
