using System.Collections.Generic;
using System.Threading.Tasks;
using AzureFunctionTier.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Extensions.Logging;

namespace AzureFunctionTier
{
    public class ExampleFunction
    {
        private readonly IRepository<Users> _repository;

        public ExampleFunction(IRepositoryFactory factory)
        {
            _repository = factory.RepositoryOf<Users>();
        }

        [FunctionName("ExampleFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            List<Users> users = (List<Users>) await _repository.GetAsync(u => u.EmailAddress == "gail.underwood@parkeeng.com");

            string responseMessage = users.Count > 0
                                         ? $"{users[0].FullName} is a registered user."
                                         : "No user in the database matching email address.";

            return new OkObjectResult(responseMessage);
        }
    }
}
