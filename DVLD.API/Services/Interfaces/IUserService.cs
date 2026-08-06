using DVLD.API.Common.QueryParameters;
using DVLD.API.Common.Results;
using DVLD.API.DTOs.Common;
using DVLD.API.DTOs.Users;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Services.Interfaces;

public interface IUserService
{
    Task<PagedResultDto<UserDto>> GetAllUsersAsync(UsersQueryParameters parameters);
    Task<UserDto?> GetUserDtoByIdAsync(int id);
    Task<ServiceResult<UserDto>> CreateUserAsync(CreateUserDto createUserDto);
    Task<ServiceResult<UserDto>> ChangeUserNameAsync(int userId, ChangeUserNameDto changeUserNameDto);
    Task<ServiceResult<UserDto>> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
    Task<ServiceResult<UserDto>> ChangeUserStatusAsync(int userId, ChangeUserStatusDto changeUserStatusDto);
}