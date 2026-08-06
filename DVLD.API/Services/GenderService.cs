using DVLD.API.DTOs.Countries;
using DVLD.API.Services.Interfaces;
using DVLD.DataAccess.Data;
using DVLD.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DVLD.API.Services;

public class GenderService : IGenderService
{

    private readonly DVLDContext _context;

    public GenderService(DVLDContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<GenderDto>> GetAllGendersAsync()
    {
        return await _context.Genders.Select(g => new GenderDto(g.GenderID, g.GenderName))
        .AsNoTracking()
        .ToListAsync();
    }

    public async Task<Gender?> GetGenderByIdAsync(int genderId)
    {
        return await _context.Genders.FindAsync(genderId);
    }

    public async Task<GenderDto?> GetGenderDtoByIdAsync(int genderId)
    {
        return await _context.Genders.AsNoTracking()
        .Where(g => g.GenderID == genderId)
        .Select(g => new GenderDto(g.GenderID, g.GenderName)).SingleOrDefaultAsync();
    }
}