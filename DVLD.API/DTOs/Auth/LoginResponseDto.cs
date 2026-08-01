using DVLD.API.DTOs.Users;

namespace DVLD.API.DTOs.Auth;

public class LoginResponseDto
{
    public UserDto User { get; set; } = null!;
    public string Token { get; set; } = null!;
}