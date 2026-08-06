namespace DVLD.API.Common.Results;

public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; private set; }

    public static ServiceResult<T> Success(T data)
    {
        return new ServiceResult<T>
        {
            IsSuccess = true,
            Data = data
        };
    }

    public new static ServiceResult<T> Failure(List<string> errors, FailureType resultType)
    {
        return new ServiceResult<T>
        {
            IsSuccess = false,
            ResultType = resultType,
            Errors = errors
        };
    }
}