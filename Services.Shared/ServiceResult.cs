namespace Services.Shared;

public class ServiceResult
{
    public string? ErrorMessage { get; }

    public bool Success => ErrorMessage is not null;

    private ServiceResult(string? errorMessage = null) =>
        ErrorMessage = errorMessage;

    public static ServiceResult Successful() => new ServiceResult();

    public static ServiceResult Failure(string error) => new ServiceResult(error);
}