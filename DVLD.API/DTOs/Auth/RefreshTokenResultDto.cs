using DVLD.DataAccess.Entities;

namespace DVLD.API.DTOs.Auth;

public class RefreshTokenResultDto
{
    public RefreshToken RefreshTokenEntity { get; set; } = null!;
    public string Token { get; set; } = null!;
}