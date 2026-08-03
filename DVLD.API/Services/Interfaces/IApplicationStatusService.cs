using DVLD.API.DTOs.ApplicationStatuses;

namespace DVLD.API.Services.Interfaces;

public interface IApplicationStatusService
{
    Task<IEnumerable<ApplicationStatusDto>> GetAllApplicationStatusesAsync();
    Task<ApplicationStatusDto?> GetApplicationStatusByIdAsync(int id);
}