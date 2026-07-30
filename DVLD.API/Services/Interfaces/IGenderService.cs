using DVLD.API.DTOs.Countries;

namespace DVLD.API.Services.Interfaces;

public interface IGenderService
{
    Task<GenderDto?> GetGenderByIdAsync(int id);
    Task<IEnumerable<GenderDto>> GetAllGendersAsync();
    Task<bool> GenderExistsAsync(int id);
}