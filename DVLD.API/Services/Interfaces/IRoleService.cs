namespace DVLD.API.Services.Interfaces;

public interface IRoleService
{
    Task<int?> GetRoleIDByRoleTitle(string title);
    Task<int?> GetEmployeeRoleIDAsync();
}