using DVLD.API.DTOs.ApplicationStatuses;

namespace DVLD.API.Services.Interfaces;

public interface IApplicationStatusService
{
    Task<List<ApplicationStatusDto>> GetAllApplicationStatusesAsync();
    Task<ApplicationStatusDto?> GetApplicationStatusByIdAsync(int applicationStatusId);
}