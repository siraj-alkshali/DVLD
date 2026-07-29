using DVLD.API.DTOs.People;

namespace DVLD.API.Services.Interfaces;

public interface IPersonService
{
    public Task<int> CreatePersonAsync(CreatePersonDto dto); // Result<T>
    public Task<PersonDto?> GetPersonByIDAsync(int id);
    public Task<IEnumerable<PersonDto>> GetAllPeopleAsync();
    public Task<bool> UpdatePersonAsync(UpdatePersonDto dto); // Result<T>
    public Task<bool> DeletePersonAsync(int id); // Result<T>
    public Task<PersonDto?> SearchPeopleByPersonID(int id);
    public Task<PersonDto?> SearchPeopleByNationalNoAsync(string nationalNo);
}