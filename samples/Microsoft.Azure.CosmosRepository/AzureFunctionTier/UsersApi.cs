// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace AzureFunctionTier;

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
        using var cancellationSource =
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

    // Query URL : http://localhost:7071/api/GetAllUsers?pageNumber=1&pageSize=25
    [FunctionName("GetAllUsers")]
    public async Task<IActionResult> GetAllUsers(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
        ILogger log,
        CancellationToken hostCancellationToken)
    {

        using var cancellationSource =
            CancellationTokenSource.CreateLinkedTokenSource(hostCancellationToken, req.HttpContext.RequestAborted);

        string pageNumber = req.Query["pageNumber"];
        string pageSize = req.Query["pageSize"];

        if (string.IsNullOrWhiteSpace(pageNumber) || !int.TryParse(pageNumber, out var page) || page <= 0)
        {
            log.LogWarning("No pageNumber provided.");
            return new BadRequestResult();
        }

        if (string.IsNullOrWhiteSpace(pageSize) || !int.TryParse(pageSize, out var size) || size <= 0)
        {
            log.LogWarning("No pageSize provided.");
            return new BadRequestResult();
        }

        IPage<User> users =
            await _repository.PageAsync(pageNumber: page, pageSize: size, cancellationToken: cancellationSource.Token);

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
        using var cancellationSource =
            CancellationTokenSource.CreateLinkedTokenSource(hostCancellationToken, req.HttpContext.RequestAborted);

        var requestBody = await new StreamReader(req.Body).ReadToEndAsync(hostCancellationToken);
        PostUserRequest userInput = JsonConvert.DeserializeObject<PostUserRequest>(requestBody);

        log.LogInformation(
            "Input (request body): {RequestBody}",
            (requestBody ?? "").Replace(Environment.NewLine, string.Empty));

        User user = await _repository.CreateAsync(userInput, cancellationSource.Token);

        return new OkObjectResult(user);
    }
}
