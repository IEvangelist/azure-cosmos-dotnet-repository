using Newtonsoft.Json;

namespace AzureFunctionTier.Model
{
    public class PostUserRequest
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("emailAddress")]
        public string EmailAddress { get; set; }

        #region Converters

        /// <summary>
        /// Create a <see cref="User"/> out of a <see cref="PostUserRequest"/>.
        /// </summary>
        /// <param name="postUserRequest">The <see cref="PostUserRequest"/> to start with.</param>
        public static implicit operator User(PostUserRequest postUserRequest)
        {
            return new User()
                   {
                       FirstName = postUserRequest.FirstName,
                       LastName = postUserRequest.LastName,
                       EmailAddress = postUserRequest.EmailAddress,
                   };
        }

        #endregion
    }
}