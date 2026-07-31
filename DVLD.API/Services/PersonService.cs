using DVLD.API.Services.Interfaces;
using DVLD.DataAccess.Data;
using DVLD.API.DTOs.People;
using DVLD.DataAccess.Entities;
using DVLD.API.Common.Results;
using Microsoft.EntityFrameworkCore;
using DVLD.API.Mappings.People;

namespace DVLD.API.Services;

public class PersonService : IPersonService
{
    private readonly DVLDContext _context;
    private readonly ICountryService _countryService;
    private readonly IGenderService _genderService;
    private readonly IImageService _imageService;

    public PersonService(DVLDContext context, ICountryService countryService, IGenderService genderService, IImageService imageService)
    {
        _context = context;
        _countryService = countryService;
        _genderService = genderService;
        _imageService = imageService;
    }

    private async Task<bool> NationalNoExistsAsync(string nationalNo)
    {
        return await _context.People.AnyAsync(p => p.NationalNo == nationalNo);
    }

    private async Task<bool> NationalNoExistsForAnotherPersonAsync(string nationalNo, int id)
    {
        return await _context.People.AnyAsync(p => p.NationalNo == nationalNo && p.PersonID != id);
    }

    private async Task<List<string>> GetReferenceErrorsAsync(int genderId, int countryId)
    {
        List<string> errors = new List<string>();

        if (!await _genderService.GenderExistsAsync(genderId))
            errors.Add("The selected gender does not exist");

        if (!await _countryService.CountryExistsAsync(countryId))
            errors.Add("The selected nationality country does not exist");

        return errors;
    }

    private async Task<List<string>> GetCreatePersonValidationErrorsAsync(CreatePersonDto createPersonDto)
    {
        List<string> errors = new List<string>();

        if (await NationalNoExistsAsync(createPersonDto.NationalNo))
            errors.Add("A person with this national number already exists");

        errors.AddRange(await GetReferenceErrorsAsync(createPersonDto.GenderID, createPersonDto.NationalityCountryID));

        return errors;
    }

    private async Task<List<string>> GetUpdatePersonValidationErrorsAsync(UpdatePersonDto updatePersonDto, int id)
    {
        List<string> errors = new List<string>();

        if (await NationalNoExistsForAnotherPersonAsync(updatePersonDto.NationalNo, id))
            errors.Add("A person with this national number already exists");

        errors.AddRange(await GetReferenceErrorsAsync(updatePersonDto.GenderID, updatePersonDto.NationalityCountryID));

        return errors;
    }

    public async Task<IEnumerable<PersonDto>> GetAllPeopleAsync()
    {
        return await _context.People
        .AsNoTracking()
        .Select(p => new PersonDto(
        p.PersonID,
        p.NationalNo,
        $"{p.FirstName} {p.SecondName} {p.ThirdName} {p.LastName}",
        p.DateOfBirth,
        p.Gender.GenderName,
        p.Address,
        p.Phone,
        p.Email,
        p.NationalityCountry.CountryName,
        p.ImagePath
    )).ToListAsync();
    }

    public async Task<PersonDto?> GetPersonByIdAsync(int id)
    {
        Person? person = await _context.People.Where(p => p.PersonID == id)
                        .AsNoTracking()
                        .Include(p => p.Gender)
                        .Include(p => p.NationalityCountry)
                        .SingleOrDefaultAsync();

        if (person == null)
            return null;

        return person.ToDto(_imageService.GetImageUrl(person.ImagePath));
    }

    public async Task<ServiceResult<PersonDto>> CreatePersonAsync(CreatePersonDto createPersonDto)
    {
        List<string> errors = await GetCreatePersonValidationErrorsAsync(createPersonDto);

        if (errors.Count > 0)
            return ServiceResult<PersonDto>.Failure(errors, FailureType.Conflict);

        Person person = createPersonDto.ToEntity();

        await _context.People.AddAsync(person);

        await _context.SaveChangesAsync();

        Person savedPerson = await _context.People.Where(p => p.PersonID == person.PersonID)
                        .AsNoTracking()
                        .Include(p => p.Gender)
                        .Include(p => p.NationalityCountry)
                        .SingleAsync();

        return ServiceResult<PersonDto>.Success(savedPerson.ToDto(_imageService.GetImageUrl(savedPerson.ImagePath)));
    }

    public async Task<ServiceResult<PersonDto>> UpdatePersonAsync(int id, UpdatePersonDto updatePersonDto)
    {

        Person? person = await _context.People.Include(p => p.Gender)
        .Include(p => p.NationalityCountry)
        .SingleOrDefaultAsync(p => p.PersonID == id);

        if (person == null)
        {
            return ServiceResult<PersonDto>.Failure(["The requested person was not found"], FailureType.NotFound);
        }

        List<string> errors = await GetUpdatePersonValidationErrorsAsync(updatePersonDto, id);

        if (errors.Count > 0)
            return ServiceResult<PersonDto>.Failure(errors, FailureType.Conflict);

        person.UpdateFromDto(updatePersonDto);

        await _context.SaveChangesAsync();

        Person updatedPerson = await _context.People.AsNoTracking()
        .Include(p => p.Gender)
        .Include(p => p.NationalityCountry)
        .SingleAsync(p => p.PersonID == person.PersonID);

        return ServiceResult<PersonDto>.Success(updatedPerson.ToDto(_imageService.GetImageUrl(updatedPerson.ImagePath)));
    }

    public async Task<ServiceResult<PersonDto>> UpdatePersonImageAsync(int personId, IFormFile image)
    {
        Person? person = await _context.People.Include(p => p.Gender)
        .Include(p => p.NationalityCountry)
        .SingleAsync(p => p.PersonID == personId);

        if (person == null)
            return ServiceResult<PersonDto>.Failure(["The requested person was not found"], FailureType.NotFound);

        ServiceResult<string> imageUploadResult = await _imageService.UploadImageAsync(image);

        if (!imageUploadResult.IsSuccess)
            return ServiceResult<PersonDto>.Failure(imageUploadResult.Errors, FailureType.Validation);

        string? oldImage = person.ImagePath;

        person.ImagePath = imageUploadResult.Data;

        await _context.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(oldImage))
            await _imageService.DeleteImage(oldImage);

        return ServiceResult<PersonDto>.Success(person.ToDto(_imageService.GetImageUrl(person.ImagePath)));
    }

    public async Task<bool> DeletePersonAsync(int id)
    {
        Person? person = await _context.People
            .FindAsync(id);

        if (person == null)
        {
            return false;
        }

        string? imagePath = person.ImagePath;

        _context.People.Remove(person);

        await _context.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(imagePath))
            await _imageService.DeleteImage(imagePath);

        return true;
    }

    public async Task<PersonDto?> GetPersonByNationalNoAsync(string nationalNo)
    {
        Person? person = await _context.People.Where(p => p.NationalNo == nationalNo)
                        .AsNoTracking()
                        .Include(p => p.Gender)
                        .Include(p => p.NationalityCountry)
                        .SingleOrDefaultAsync();

        if (person == null)
            return null;

        return person.ToDto(_imageService.GetImageUrl(person.ImagePath));
    }

    public async Task<bool> PersonExistsAsync(int id)
    {
        return await _context.People.AnyAsync(p => p.PersonID == id);
    }
}