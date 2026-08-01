using DVLD.API.Common.Results;
using DVLD.API.DTOs.Auth;
using DVLD.API.Common;
using DVLD.API.Services.Interfaces;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using DVLD.API.Mappings.Users;
using DVLD.API.DTOs.Users;

namespace DVLD.API.Services;

public class AuthService : IAuthService
{
    private readonly DVLDContext _context;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthService(DVLDContext context, IPasswordHasherService passwordHasherService, IJwtService jwtService, IRefreshTokenService refreshTokenService)
    {
        _context = context;
        _passwordHasherService = passwordHasherService;
        _jwtService = jwtService;
        _refreshTokenService = refreshTokenService;
    }

    private async Task<User?> GetUserByUserNameAsync(string userName)
    {
        return await _context.Users.AsNoTracking()
        .Include(u => u.Person)
        .Include(u => u.Role)
        .SingleOrDefaultAsync(u => u.UserName == userName);
    }

    public async Task<ServiceResult<LoginResponseDto>> LoginAsync(LoginRequestDto dto)
    {
        string normalizedUserName = StringUtilities.NormalizeUserName(dto.UserName);

        User? user = await GetUserByUserNameAsync(normalizedUserName);

        if (user == null || !_passwordHasherService.VerifyPassword(dto.Password, user.PasswordHash))
            return ServiceResult<LoginResponseDto>.Failure(["Invalid credentials"], FailureType.Unauthorized);

        if (!user.IsActive)
            return ServiceResult<LoginResponseDto>.Failure(["This user is not active"], FailureType.Forbidden);

        string token = _jwtService.GenerateToken(user);

        RefreshTokenResultDto refreshTokenResult = await _refreshTokenService.CreateRefreshTokenAsync(user.UserID);

        return ServiceResult<LoginResponseDto>.Success(new LoginResponseDto
        {
            User = user.ToDto(),
            AccessToken = token,
            RefreshToken = refreshTokenResult.Token
        });
    }

    public async Task<ServiceResult<LoginResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto dto)
    {
        RefreshToken? refreshToken = await _refreshTokenService.GetRefreshTokenAsync(dto.RefreshToken);

        if (refreshToken == null)
            return ServiceResult<LoginResponseDto>.Failure(["Invalid refresh token"], FailureType.Unauthorized);

        if (!_refreshTokenService.IsRefreshTokenValidAsync(refreshToken))
            return ServiceResult<LoginResponseDto>.Failure(["Refresh token expired or revoked"], FailureType.Unauthorized);

        User user = refreshToken.User;

        string newAccessToken = _jwtService.GenerateToken(user);
        RefreshTokenResultDto newRefreshTokenResult = await _refreshTokenService.CreateRefreshTokenAsync(user.UserID);

        await _refreshTokenService.RevokeRefreshTokenAsync(refreshToken, newRefreshTokenResult.RefreshTokenEntity.TokenHash);

        return ServiceResult<LoginResponseDto>.Success(new LoginResponseDto
        {
            User = user.ToDto(),
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenResult.Token
        });
    }

    public async Task<ServiceResult<bool>> LogoutAsync(LogoutRequestDto dto)
    {
        RefreshToken? refreshToken = await _refreshTokenService.GetRefreshTokenAsync(dto.RefreshToken);

        if (refreshToken == null)
            return ServiceResult<bool>.Failure(["Invalid refresh token"], FailureType.Unauthorized);

        await _refreshTokenService.RevokeRefreshTokenAsync(refreshToken);

        return ServiceResult<bool>.Success(true);
    }

    public async Task<UserDto?> GetUserByUserIdAsync(int userId)
    {
        User? user = await _context.Users
            .AsNoTracking()
            .Include(u => u.Person)
            .Include(u => u.Role)
            .SingleOrDefaultAsync(u => u.UserID == userId);

        if (user == null)
            return null;

        return user.ToDto();
    }


}