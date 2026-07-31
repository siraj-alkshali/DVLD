using DVLD.API.DTOs.Users;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Mappings.Users;

public static class UserMappingExtensions
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto(
            user.UserID,
            $"{user.Person.FirstName} {user.Person.LastName}",
            user.UserName
        );
    }
}