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

    private async Task<ServiceResult<User>> ValidateLoginCredentialsAsync(string userName, string password)
    {
        User? user = await GetUserByUserNameAsync(userName);

        if (user == null || !_passwordHasherService.VerifyPassword(password, user.PasswordHash))
            return ServiceResult<User>.Failure(["Invalid credentials"], FailureType.Unauthorized);

        if (!user.IsActive)
            return ServiceResult<User>.Failure(["This user is not active"], FailureType.Forbidden);

        return ServiceResult<User>.Success(user);
    }

    private async Task<ServiceResult<RefreshToken>> ValidateRefreshTokenAsync(string rawRefreshToken)
    {
        RefreshToken? refreshToken = await _refreshTokenService.GetRefreshTokenAsync(rawRefreshToken);

        if (refreshToken == null)
            return ServiceResult<RefreshToken>.Failure(["Invalid refresh token"], FailureType.Unauthorized);

        if (!_refreshTokenService.IsRefreshTokenValidAsync(refreshToken))
            return ServiceResult<RefreshToken>.Failure(["Refresh token expired or revoked"], FailureType.Unauthorized);

        return ServiceResult<RefreshToken>.Success(refreshToken);
    }

    public async Task<ServiceResult<LoginResponseDto>> LoginAsync(LoginRequestDto loginRequestDto)
    {
        string normalizedUserName = StringUtilities.NormalizeUserName(loginRequestDto.UserName);

        ServiceResult<User> loginRequestDtoValidation = await ValidateLoginCredentialsAsync(normalizedUserName, loginRequestDto.Password);

        if (!loginRequestDtoValidation.IsSuccess)
            return ServiceResult<LoginResponseDto>.Failure(loginRequestDtoValidation.Errors, loginRequestDtoValidation.ResultType!.Value);

        User user = loginRequestDtoValidation.Data!;

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
        ServiceResult<RefreshToken> refreshTokenRequestValidation = await ValidateRefreshTokenAsync(dto.RefreshToken);

        if (!refreshTokenRequestValidation.IsSuccess)
            return ServiceResult<LoginResponseDto>.Failure(refreshTokenRequestValidation.Errors, refreshTokenRequestValidation.ResultType!.Value);

        RefreshToken refreshToken = refreshTokenRequestValidation.Data!;

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

    public async Task<UserDto?> GetUserDtoByUserIdAsync(int userId)
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