using DVLD.DataAccess.Entities;

namespace DVLD.API.DTOs.Auth;

public class LogoutRequestDto
{
    public string RefreshToken { get; set; } = null!;
}