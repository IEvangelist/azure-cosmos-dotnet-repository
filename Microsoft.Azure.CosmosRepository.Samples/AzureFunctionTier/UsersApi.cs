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
using System.Threading;

namespace AzureFunctionTier
{
    public class UsersApi
    {
        readonly IRepository<User> _repository;

        public UsersApi(IRepositoryFactory factory) => _repository = factory.RepositoryOf<User>();

        // Query URL : http://localhost:7071/api/GetUsers?email=bill.gates@microsoft.com
        [FunctionName("GetUsers")]
        public async Task<IActionResult> GetUsers(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log,
            CancellationToken hostCancellationToken)
        {
            using CancellationTokenSource cancellationSource =
                CancellationTokenSource.CreateLinkedTokenSource(hostCancellationToken, req.HttpContext.RequestAborted);

            string emailAddress = req.Query["email"];

            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                log.LogWarning("No email address provided.");
                return new BadRequestResult();
            }

            IEnumerable<User> users =
                await _repository.GetAsync(u => u.EmailAddress == emailAddress, cancellationSource.Token);

            return new OkObjectResult(users);
        }

        // Query URL : http://localhost:7071/api/PostUser
        // Body :
        //  {
        //    "firstName":"Bill",
        //    "lastName":"Gates",
        //    "emailAddress":"bill.gates@microsoft.com",
        //  }
        [FunctionName("PostUser")]
        public async Task<IActionResult> PostUser(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log,
            CancellationToken hostCancellationToken)
        {
            using CancellationTokenSource cancellationSource =
                CancellationTokenSource.CreateLinkedTokenSource(hostCancellationToken, req.HttpContext.RequestAborted);

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            PostUserRequest userInput = JsonConvert.DeserializeObject<PostUserRequest>(requestBody);

            log.LogInformation($"Input: {userInput}");

            User user = await _repository.CreateAsync(userInput, cancellationSource.Token);

            return new OkObjectResult(user);
        }
    }
}
