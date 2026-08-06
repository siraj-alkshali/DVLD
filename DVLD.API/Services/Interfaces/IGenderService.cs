using DVLD.API.DTOs.Countries;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Services.Interfaces;

public interface IGenderService
{
    Task<IEnumerable<GenderDto>> GetAllGendersAsync();
    Task<Gender?> GetGenderByIdAsync(int genderId);
    Task<GenderDto?> GetGenderDtoByIdAsync(int genderId);
}