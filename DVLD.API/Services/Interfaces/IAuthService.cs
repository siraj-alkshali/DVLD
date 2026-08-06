using DVLD.API.Common.Results;
using DVLD.API.DTOs.Auth;
using DVLD.API.DTOs.Users;

namespace DVLD.API.Services.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<LoginResponseDto>> LoginAsync(LoginRequestDto loginRequestDto);
    Task<UserDto?> GetUserDtoByUserIdAsync(int userId);
    Task<ServiceResult<LoginResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequestDto);
    Task<ServiceResult<bool>> LogoutAsync(LogoutRequestDto logoutRequestDto);
}