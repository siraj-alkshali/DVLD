using DVLD.API.DTOs.Users;

namespace DVLD.API.DTOs.Auth;

public class LoginResponseDto
{
    public UserDto User { get; set; } = null!;
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}