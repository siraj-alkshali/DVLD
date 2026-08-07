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

    public async Task<TestTypeDto?> GetTestTypeDtoByIdAsync(int testTypeId)
    {
        return await _context.TestTypes.Where(tt => tt.TestTypeID == testTypeId)
        .Select(tt => new TestTypeDto(
        tt.TestTypeID, tt.TestTypeTitle, tt.TestTypeDescription, tt.TestTypeFees
        )).SingleOrDefaultAsync();
    }

    public async Task<TestType?> GetTestTypeByIdAsync(int testTypeId)
    {
        return await _context.TestTypes.FindAsync(testTypeId);
    }

    public async Task<bool> TestTypeExistsAsync(int testTypeId)
    {
        return await _context.TestTypes.AnyAsync(tt => tt.TestTypeID == testTypeId);
    }
}