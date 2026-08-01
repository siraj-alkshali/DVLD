using DVLD.API.DTOs.Auth;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Services.Interfaces;

public interface IRefreshTokenService
{
    Task<RefreshTokenResultDto> CreateRefreshTokenAsync(int userId);
    Task<RefreshToken?> GetRefreshTokenAsync(string token);
    bool IsRefreshTokenValidAsync(RefreshToken refreshToken);
    Task RevokeRefreshTokenAsync(RefreshToken refreshToken, string? replacedByTokenHash = null);
}