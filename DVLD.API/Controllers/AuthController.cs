using DVLD.API.Common.Results;
using DVLD.API.DTOs.Auth;
using DVLD.API.DTOs.Users;
using DVLD.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using DVLD.API.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace DVLD.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICurrentUserService _currentUserService;

    public AuthController(IAuthService authService, ICurrentUserService currentUserService)
    {
        _authService = authService;
        _currentUserService = currentUserService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto dto)
    {
        ServiceResult<LoginResponseDto> result = await _authService.LoginAsync(dto);

        if (!result.IsSuccess)
            return this.ToActionResult(result);

        return Ok(result.Data);
    }

    [Authorize]
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> Refresh(RefreshTokenRequestDto dto)
    {
        ServiceResult<LoginResponseDto> result = await _authService.RefreshTokenAsync(dto);

        if (!result.IsSuccess)
            return this.ToActionResult(result);

        return Ok(result.Data);
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        UserDto? user = await _authService.GetUserDtoByUserIdAsync(_currentUserService.UserID);

        if (user == null)
            return Unauthorized();

        return Ok(user);
    }

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout(LogoutRequestDto dto)
    {
        ServiceResult<bool> result = await _authService.LogoutAsync(dto);

        if (!result.IsSuccess)
            return this.ToActionResult(result);

        return Ok(result.Data);
    }
}