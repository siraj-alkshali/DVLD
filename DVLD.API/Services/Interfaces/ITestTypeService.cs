using DVLD.API.DTOs.TestTypes;

namespace DVLD.API.Services.Interfaces;

public interface ITestTypeService
{
    Task<IEnumerable<TestTypeDto>> GetAllTestTypesAsync();
    Task<TestTypeDto?> GetTestTypeByIdAsync(int id);
}