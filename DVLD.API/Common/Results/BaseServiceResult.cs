namespace DVLD.API.Common.Results;

public class ServiceResult
{
    public bool IsSuccess { get; protected set; }
    public FailureType? ResultType { get; protected set; }
    public List<string> Errors { get; protected set; } = new List<string>();

    public static ServiceResult Success()
    {
        return new ServiceResult
        {
            IsSuccess = true
        };
    }

    public static ServiceResult Failure(List<string> errors, FailureType type)
    {
        return new ServiceResult
        {
            IsSuccess = false,
            Errors = errors,
            ResultType = type
        };
    }
}