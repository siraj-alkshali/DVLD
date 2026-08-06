namespace DVLD.API.Common.Results;

public enum FailureType
{
    ValidationError,
    NotFound,
    Conflict,
    Unauthorized,
    Forbidden,
    BadRequest,
    InternalServerError
}