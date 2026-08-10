using DVLD.DataAccess.Entities;
using DVLD.API.Common.Results;
using DVLD.API.DTOs;
using DVLD.API.DTOs.Common;
using DVLD.API.Common.QueryParameters;

namespace DVLD.API.Services.Interfaces;

public interface IDriverService
{
    Task<(Driver Driver, bool IsNew)> GetOrCreateDriverAsync(int personId);
    Task<PagedResultDto<DriverDto>> GetAllDriversAsync(DriversQueryParameters parameters);
}