using DVLD.API.DTOs.TestTypes;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Services.Interfaces;

public interface ITestTypeService
{
    Task<List<TestTypeDto>> GetAllTestTypesAsync();
    Task<TestTypeDto?> GetTestTypeDtoByIdAsync(int testTypeId);
    Task<TestType?> GetTestTypeByIdAsync(int testTypeId);
}