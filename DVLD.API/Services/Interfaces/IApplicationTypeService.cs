using DVLD.API.DTOs.ApplicationTypes;

namespace DVLD.API.Services.Interfaces;

public interface IApplicationTypeService
{
    Task<IEnumerable<ApplicationTypeDto>> GetAllApplicationTypesAsync();
    Task<ApplicationTypeDto?> GetApplicationTypeByIdAsync(int id);
    Task<bool> ApplicationTypeExists(int id);
}