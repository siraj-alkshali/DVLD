namespace DVLD.API.Services.Interfaces;

public interface IRoleService
{
    Task<bool> RoleExistsAsync(int id);
    Task<int?> GetRoleIDByRoleTitle(string title);
    Task<int?> GetEmployeeRoleIDAsync();
}