namespace DVLD.API.DTOs.Users;

public class CreateUserDto
{
    public int PersonID { get; set; }
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
}