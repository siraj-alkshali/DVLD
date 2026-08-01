using DVLD.API.Services.Interfaces;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DVLD.API.Services;

public class RoleService : IRoleService
{
    private readonly DVLDContext _context;

    public RoleService(DVLDContext context)
    {
        _context = context;
    }

    public async Task<int?> GetRoleIDByRoleTitle(string title)
    {
        return await _context.Roles.Where(r => r.RoleTitle == title)
        .Select(r => r.RoleID)
        .SingleOrDefaultAsync();
    }

    public async Task<int?> GetEmployeeRoleIDAsync()
    {
        return await _context.Roles.Where(r => r.RoleTitle == "Employee")
        .Select(r => r.RoleID)
        .SingleOrDefaultAsync();
    }

    public async Task<bool> RoleExistsAsync(int id)
    {
        return await _context.Roles.AnyAsync(r => r.RoleID == id);
    }
}