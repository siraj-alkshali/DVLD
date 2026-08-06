using DVLD.API.DTOs.People;
using DVLD.API.Common.Results;
using DVLD.API.DTOs.Common;
using DVLD.API.Common.QueryParameters;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Services.Interfaces;

public interface IPersonService
{
    Task<PagedResultDto<PersonDto>> GetAllPeopleAsync(PeopleQueryParameters parameters);
    Task<Person?> GetPersonByIdAsync(int personId);
    Task<Person?> GetPersonByIdWithDetailsAsync(int personId);
    Task<PersonDto?> GetPersonDtoByIdAsync(int personId);
    Task<ServiceResult<PersonDto>> CreatePersonAsync(CreatePersonDto createPersonDto);
    Task<ServiceResult<PersonDto>> UpdatePersonAsync(int id, UpdatePersonDto updatePersonDto);
    Task<bool> DeletePersonAsync(int personId);
    Task<PersonDto?> GetPersonDtoByNationalNoAsync(string nationalNo);
    Task<ServiceResult<PersonDto>> UpdatePersonImageAsync(int personId, IFormFile image);
    Task<bool> PersonExistsAsync(int personId);
}