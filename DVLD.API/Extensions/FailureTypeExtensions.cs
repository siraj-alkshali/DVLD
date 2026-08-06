using Microsoft.AspNetCore.Mvc;
using DVLD.API.Common.Results;

namespace DVLD.API.Extensions;

public static class FailureTypeExtensions
{
    public static ActionResult ToActionResult<T>(this ControllerBase controller, ServiceResult<T> result)
    {
        return result.ResultType switch
        {
            FailureType.ValidationError =>
                controller.BadRequest(result.Errors),

            FailureType.BadRequest =>
                controller.BadRequest(result.Errors),

            FailureType.NotFound =>
                controller.NotFound(result.Errors),

            FailureType.Conflict =>
                controller.Conflict(result.Errors),

            FailureType.Unauthorized =>
                controller.Unauthorized(result.Errors),

            FailureType.Forbidden =>
                controller.StatusCode(
                    StatusCodes.Status403Forbidden,
                    result.Errors),

            FailureType.InternalServerError =>
                controller.StatusCode(
                    StatusCodes.Status500InternalServerError),

            _ =>
                controller.StatusCode(
                    StatusCodes.Status500InternalServerError)
        };
    }
}