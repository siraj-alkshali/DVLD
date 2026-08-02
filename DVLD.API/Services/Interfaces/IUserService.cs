using DVLD.API.Common.QueryParameters;
using DVLD.API.Common.Results;
using DVLD.API.DTOs.Common;
using DVLD.API.DTOs.Users;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Services.Interfaces;

public interface IUserService
{
    Task<PagedResultDto<UserDto>> GetAllUsersAsync(UsersQueryParameters parameters);
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<ServiceResult<UserDto>> CreateUserAsync(CreateUserDto createUserDto);
    Task<bool> UserExistsAsync(int id);
    Task<ServiceResult<UserDto>> ChangeUserNameAsync(int id, ChangeUserNameDto dto);
    Task<ServiceResult<UserDto>> ChangePasswordAsync(int id, ChangePasswordDto dto);
    Task<ServiceResult<UserDto>> ChangeUserStatusAsync(int id, ChangeUserStatusDto dto);
}