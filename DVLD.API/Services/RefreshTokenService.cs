using System.Security.Cryptography;
using System.Text;
using DVLD.API.Services.Interfaces;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Entities;
using DVLD.API.DTOs.Auth;
using Microsoft.EntityFrameworkCore;

namespace DVLD.API.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly DVLDContext _context;

    public RefreshTokenService(DVLDContext context)
    {
        _context = context;
    }

    private string HashToken(string token)
    {
        using SHA256 sha256 = SHA256.Create();

        byte[] bytes = Encoding.UTF8.GetBytes(token);

        byte[] hash = sha256.ComputeHash(bytes);

        return Convert.ToBase64String(hash);
    }

    public async Task<RefreshTokenResultDto> CreateRefreshTokenAsync(int userId)
    {
        byte[] randomBytes = RandomNumberGenerator.GetBytes(64);

        string token = Convert.ToBase64String(randomBytes);

        RefreshToken refreshTokenEntity = new()
        {
            UserID = userId,
            TokenHash = HashToken(token),
            CreatedDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(7)
        };

        _context.RefreshTokens.Add(refreshTokenEntity);

        await _context.SaveChangesAsync();

        return new RefreshTokenResultDto
        {
            RefreshTokenEntity = refreshTokenEntity,
            Token = token
        };
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        string tokenHash = HashToken(token);

        return await _context.RefreshTokens
        .Include(rt => rt.User)
        .ThenInclude(u => u.Person)
        .Include(rt => rt.User)
        .ThenInclude(u => u.Role)
        .SingleOrDefaultAsync(rt => rt.TokenHash == tokenHash);
    }

    public bool IsRefreshTokenValidAsync(RefreshToken refreshToken)
    {
        return refreshToken.RevokedDate == null && refreshToken.ExpiryDate > DateTime.UtcNow;
    }

    public async Task RevokeRefreshTokenAsync(RefreshToken refreshToken, string? replacedByTokenHash = null)
    {
        refreshToken.RevokedDate = DateTime.UtcNow;
        refreshToken.ReplacedByTokenHash = replacedByTokenHash;

        await _context.SaveChangesAsync();
    }
}