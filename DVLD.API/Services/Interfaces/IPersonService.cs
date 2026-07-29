using DVLD.API.DTOs.People;
using DVLD.API.Common.Results;

namespace DVLD.API.Services.Interfaces;

public interface IPersonService
{
    public Task<IEnumerable<PersonDto>> GetAllPeopleAsync();
    public Task<PersonDto?> GetPersonByIdAsync(int id);
    public Task<ServiceResult<PersonDto>> CreatePersonAsync(CreatePersonDto dto);
    public Task<bool> UpdatePersonAsync(int id, UpdatePersonDto dto);
    public Task<bool> DeletePersonAsync(int id);
    public Task<PersonDto?> GetPersonByNationalNoAsync(string nationalNo);
}