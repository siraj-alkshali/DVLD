using DVLD.API.Common.Results;
using DVLD.API.DTOs.Auth;
using DVLD.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DVLD.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto dto)
    {
        ServiceResult<LoginResponseDto> result = await _authService.LoginAsync(dto);

        if (!result.IsSuccess)
        {
            switch (result.ResultType)
            {
                case FailureType.Unauthorized:
                    return Unauthorized(result.Errors);

                case FailureType.Forbidden:
                    return StatusCode(StatusCodes.Status403Forbidden, result.Errors);

                default:
                    return StatusCode(500);
            }
        }

        return Ok(result.Data);
    }
}