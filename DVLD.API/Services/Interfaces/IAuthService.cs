using DVLD.API.Common.Results;
using DVLD.API.DTOs.Auth;
using DVLD.API.DTOs.Users;

namespace DVLD.API.Services.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<LoginResponseDto>> LoginAsync(LoginRequestDto dto);
    Task<UserDto?> GetUserByUserIdAsync(int userId);
    Task<ServiceResult<LoginResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto dto);
    Task<ServiceResult<bool>> LogoutAsync(LogoutRequestDto dto);
}