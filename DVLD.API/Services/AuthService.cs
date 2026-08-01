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

    public AuthService(DVLDContext context, IPasswordHasherService passwordHasherService, IJwtService jwtService)
    {
        _context = context;
        _passwordHasherService = passwordHasherService;
        _jwtService = jwtService;
    }

    private async Task<User?> GetUserByUserNameAsync(string userName)
    {
        return await _context.Users.AsNoTracking()
        .Include(u => u.Person)
        .Include(u => u.Role)
        .SingleOrDefaultAsync(u => u.UserName == userName);
    }

    private string NormalizeUserName(string userName)
    {
        return userName.Trim().ToLower();
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

        return ServiceResult<LoginResponseDto>.Success(new LoginResponseDto
        {
            User = user.ToDto(),
            Token = token
        });
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