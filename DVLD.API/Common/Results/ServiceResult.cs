namespace DVLD.API.Common.Results;

public class ServiceResult<T>
{

    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public FailureType? ResultType { get; private set; }
    public List<string> Errors { get; private set; } = new List<string>();

    public static ServiceResult<T> Success(T data)
    {
        return new ServiceResult<T>
        {
            IsSuccess = true,
            Data = data
        };
    }

    public static ServiceResult<T> Failure(List<string> errors, FailureType resultType)
    {
        return new ServiceResult<T>
        {
            IsSuccess = false,
            ResultType = resultType,
            Errors = errors
        };
    }
}