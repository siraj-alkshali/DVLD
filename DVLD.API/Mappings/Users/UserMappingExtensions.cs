using DVLD.API.Common.Constants;
using DVLD.API.DTOs.Users;
using DVLD.DataAccess.Entities;
using DVLD.API.Extensions;

namespace DVLD.API.Mappings.Users;

public static class UserMappingExtensions
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto(
            user.UserID,
            $"{user.Person.FirstName} {user.Person.LastName}",
            user.UserName,
            ((enRoleType)user.RoleID).GetDisplayName()
        );
    }

    public static User ToEntity(this CreateUserDto createUserDto, string userName, string passwordHash, enRoleType roleType, Person person)
    {
        return new User
        {
            PersonID = createUserDto.PersonID,
            UserName = userName,
            PasswordHash = passwordHash,
            IsActive = true,
            RoleID = (int)roleType,
            Person = person
        };
    }
}