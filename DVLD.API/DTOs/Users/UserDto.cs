namespace DVLD.API.DTOs.Users;

public class UserDto
{
    public int UserID { get; set; }
    public string Name { get; set; } = null!;
    public string UserName { get; set; } = null!;

    public UserDto(int userID, string name, string userName)
    {
        UserID = userID;
        Name = name;
        UserName = userName;
    }
}