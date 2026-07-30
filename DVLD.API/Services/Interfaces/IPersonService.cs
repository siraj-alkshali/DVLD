using DVLD.API.DTOs.People;
using DVLD.API.Common.Results;

namespace DVLD.API.Services.Interfaces;

public interface IPersonService
{
    Task<IEnumerable<PersonDto>> GetAllPeopleAsync();
    Task<PersonDto?> GetPersonByIdAsync(int id);
    Task<ServiceResult<PersonDto>> CreatePersonAsync(CreatePersonDto dto);
    Task<ServiceResult<PersonDto>> UpdatePersonAsync(int id, UpdatePersonDto dto);
    Task<bool> DeletePersonAsync(int id);
    Task<PersonDto?> GetPersonByNationalNoAsync(string nationalNo);
}