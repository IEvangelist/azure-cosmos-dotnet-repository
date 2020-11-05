using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AzureFunctionTier.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctionTier
{
    public class UsersApi
    {
        private readonly IRepository<User> _repository;

        public UsersApi(IRepositoryFactory factory)
        {
            _repository = factory.RepositoryOf<User>();
        }

        /*
         * Query URL : http://localhost:7071/api/GetUsers?email=bill.gates@microsoft.com
         */
        [FunctionName("GetUsers")]
        public async Task<IActionResult> GetUsers(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            string emailAddress = req.Query["email"];

            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                return new BadRequestResult();
            }

            IEnumerable<User> users = await _repository.GetAsync(u => u.EmailAddress == emailAddress);

            return new OkObjectResult(users);
        }

        /*
         * Query URL : http://localhost:7071/api/PostUser
         * Body :
            {
              "firstName":"Bill",
              "lastName":"Gates",
              "emailAddress":"bill.gates@microsoft.com",
            }
         */
        [FunctionName("PostUser")]
        public async Task<IActionResult> PostUser(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            PostUserRequest userInput = JsonConvert.DeserializeObject<PostUserRequest>(requestBody);

            User user = await _repository.CreateAsync(userInput);

            return new OkObjectResult(user);
        }
    }
}
