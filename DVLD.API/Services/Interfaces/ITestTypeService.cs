using DVLD.API.DTOs.TestTypes;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Services.Interfaces;

public interface ITestTypeService
{
    Task<IEnumerable<TestTypeDto>> GetAllTestTypesAsync();
    Task<TestTypeDto?> GetTestTypeDtoByIdAsync(int id);
    Task<TestType?> GetTestTypeByIdAsync(int id);
}