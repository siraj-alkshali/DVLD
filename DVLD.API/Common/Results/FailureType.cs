namespace DVLD.API.Common.Results;

public enum FailureType
{
    NotFound,
    Unauthorized,
    Forbidden,
    Conflict,
    Validation,
    InternalError
}