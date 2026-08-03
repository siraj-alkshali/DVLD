using DVLD.API.DTOs.ApplicationTypes;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Services.Interfaces;

public interface IApplicationTypeService
{
    Task<IEnumerable<ApplicationTypeDto>> GetAllApplicationTypesAsync();
    Task<ApplicationTypeDto?> GetApplicationTypeDtoByIdAsync(int id);
    Task<ApplicationType?> GetApplicationTypeByIdAsync(int id);
}