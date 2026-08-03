using DVLD.API.Common.Constants;
using DVLD.API.DTOs.TestTypes;
using DVLD.API.Services.Interfaces;
using DVLD.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Services;

public class TestTypeService : ITestTypeService
{
    private readonly DVLDContext _context;

    public TestTypeService(DVLDContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TestTypeDto>> GetAllTestTypesAsync()
    {
        return await _context.TestTypes.Select(tt => new TestTypeDto(
        tt.TestTypeID, tt.TestTypeTitle, tt.TestTypeDescription, tt.TestTypeFees
        )).ToListAsync();
    }

    public async Task<TestTypeDto?> GetTestTypeDtoByIdAsync(int id)
    {
        return await _context.TestTypes.Where(tt => tt.TestTypeID == id)
        .Select(tt => new TestTypeDto(
        tt.TestTypeID, tt.TestTypeTitle, tt.TestTypeDescription, tt.TestTypeFees
        )).SingleOrDefaultAsync();
    }

    public async Task<TestType?> GetTestTypeByIdAsync(int id)
    {
        return await _context.TestTypes.FindAsync(id);
    }
}