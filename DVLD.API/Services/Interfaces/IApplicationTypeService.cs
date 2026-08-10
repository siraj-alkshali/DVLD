using DVLD.API.Common.Constants;
using DVLD.API.DTOs.ApplicationTypes;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Services.Interfaces;

public interface IApplicationTypeService
{
    Task<List<ApplicationTypeDto>> GetAllApplicationTypesAsync();
    Task<ApplicationTypeDto?> GetApplicationTypeDtoByIdAsync(int applicationTypeId);
    Task<decimal> GetApplicationTypeFeesAsync(enApplicationType applicationType);
}