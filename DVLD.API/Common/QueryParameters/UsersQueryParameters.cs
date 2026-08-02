namespace DVLD.API.Common.QueryParameters;

public class UsersQueryParameters : SearchParameters
{
    public bool? IsActive { get; set; }
    public int? RoleID { get; set; }
}