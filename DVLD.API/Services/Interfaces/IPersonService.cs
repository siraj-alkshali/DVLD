using DVLD.API.DTOs.People;
using DVLD.API.Common.Results;
using DVLD.API.DTOs.Common;
using DVLD.API.Common.QueryParameters;

namespace DVLD.API.Services.Interfaces;

public interface IPersonService
{
    Task<PagedResultDto<PersonDto>> GetAllPeopleAsync(PeopleQueryParameters parameters);
    Task<PersonDto?> GetPersonByIdAsync(int id);
    Task<ServiceResult<PersonDto>> CreatePersonAsync(CreatePersonDto dto);
    Task<ServiceResult<PersonDto>> UpdatePersonAsync(int id, UpdatePersonDto dto);
    Task<bool> DeletePersonAsync(int id);
    Task<PersonDto?> GetPersonByNationalNoAsync(string nationalNo);
    Task<ServiceResult<PersonDto>> UpdatePersonImageAsync(int personId, IFormFile image);
    Task<bool> PersonExistsAsync(int id);
}