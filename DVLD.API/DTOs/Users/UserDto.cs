namespace DVLD.API.DTOs.Users;

public class UserDto
{
    public int UserID { get; set; }
    public string Name { get; set; }
    public string UserName { get; set; }
    public string RoleTitle { get; set; }

    public UserDto(int userID, string name, string userName, string roleTitle)
    {
        UserID = userID;
        Name = name;
        UserName = userName;
        RoleTitle = roleTitle;
    }
}