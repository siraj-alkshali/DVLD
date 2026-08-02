using DVLD.API.DTOs.Users;
using DVLD.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using DVLD.API.Common.Results;
using DVLD.API.DTOs.Common;
using DVLD.API.Common.QueryParameters;

namespace DVLD.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<UserDto>>> GetAllUsers([FromQuery] UsersQueryParameters parameters)
    {
        return Ok(await _userService.GetAllUsersAsync(parameters));
    }

    [HttpGet("{id}", Name = "GetUserById")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUserByUserId(int id)
    {
        UserDto? user = await _userService.GetUserByIdAsync(id);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto createUserDto)
    {
        ServiceResult<UserDto> result = await _userService.CreateUserAsync(createUserDto);

        if (!result.IsSuccess)
        {
            switch (result.ResultType)
            {
                case FailureType.Conflict:
                    return Conflict(result.Errors);

                default:
                    return StatusCode(500);
            }
        }

        return CreatedAtRoute(
            "GetUserById",
            new { id = result.Data!.UserID },
            result.Data
        );
    }

    [HttpPatch("{id}/username")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> ChangeUserName(int id, ChangeUserNameDto changeUserNameDto)
    {
        ServiceResult<UserDto> result = await _userService.ChangeUserNameAsync(id, changeUserNameDto);

        if (!result.IsSuccess)
        {
            switch (result.ResultType)
            {
                case FailureType.NotFound:
                    return NotFound(result.Errors);

                case FailureType.Conflict:
                    return Conflict(result.Errors);

                default:
                    return StatusCode(500);
            }
        }

        return Ok(result.Data);
    }

    [HttpPatch("{id}/password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserDto>> ChangePassword(int id, ChangePasswordDto changePasswordDto)
    {
        ServiceResult<UserDto> result = await _userService.ChangePasswordAsync(id, changePasswordDto);

        if (!result.IsSuccess)
        {
            switch (result.ResultType)
            {
                case FailureType.NotFound:
                    return NotFound(result.Errors);

                case FailureType.Conflict:
                    return Conflict(result.Errors);

                case FailureType.Validation:
                    return BadRequest(result.Errors);

                default:
                    return StatusCode(500);
            }
        }

        return Ok(result.Data);
    }

    [HttpPatch("{id}/is-active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserDto>> ChangeUserStatus(int id, ChangeUserStatusDto dto)
    {
        ServiceResult<UserDto> result = await _userService.ChangeUserStatusAsync(id, dto);

        if (!result.IsSuccess)
        {
            switch (result.ResultType)
            {
                case FailureType.NotFound:
                    return NotFound(result.Errors);

                case FailureType.Conflict:
                    return Conflict(result.Errors);

                default:
                    return StatusCode(500);
            }
        }

        return Ok(result.Data);
    }
}