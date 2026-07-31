using DVLD.API.Common.Results;
using DVLD.API.DTOs.Users;

namespace DVLD.API.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<UserDto?> GetUserByUserNameAsync(string userName);
    Task<ServiceResult<UserDto>> CreateUserAsync(CreateUserDto createUserDto);
    Task<bool> UserExistsAsync(int id);
    Task<ServiceResult<UserDto>> ChangeUserNameAsync(int id, ChangeUserNameDto dto);
    Task<ServiceResult<UserDto>> ChangePasswordAsync(int id, ChangePasswordDto dto);
}