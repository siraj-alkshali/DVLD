using DVLD.DataAccess.Entities;
using DVLD.API.Common.Results;

namespace DVLD.API.Services.Interfaces;

public interface IDriverService
{
    Task<(Driver Driver, bool IsNew)> GetOrCreateDriverAsync(int personId);
}