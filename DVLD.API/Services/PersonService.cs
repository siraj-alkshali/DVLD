using DVLD.API.Services.Interfaces;
using DVLD.DataAccess.Data;
using DVLD.API.DTOs.People;
using DVLD.DataAccess.Entities;
using DVLD.API.Common.Results;
using Microsoft.EntityFrameworkCore;
using DVLD.API.Mappings.People;
using DVLD.API.DTOs.Common;
using DVLD.API.Common.QueryParameters;
using DVLD.API.Extensions;

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

    private async Task<bool> NationalNoExistsForAnotherPersonAsync(string nationalNo, int personId)
    {
        return await _context.People.AnyAsync(p => p.NationalNo == nationalNo && p.PersonID != personId);
    }

    private async Task<ServiceResult<(Gender gender, Country country)>> GetValidatedReferenceDataAsync(int genderId, int countryId)
    {
        Gender? gender = await _genderService.GetGenderByIdAsync(genderId);

        if (gender == null)
            return ServiceResult<(Gender gender, Country country)>.Failure(["The selected gender does not exist"], FailureType.NotFound);

        Country? country = await _countryService.GetCountryByIdAsync(countryId);

        if (country == null)
            return ServiceResult<(Gender gender, Country country)>.Failure(["The selected nationality country does not exist"], FailureType.NotFound);

        return ServiceResult<(Gender gender, Country country)>.Success((gender, country));
    }

    private async Task<ServiceResult<(Gender gender, Country country)>> ValidateCreatePersonAsync(CreatePersonDto createPersonDto)
    {
        if (await NationalNoExistsAsync(createPersonDto.NationalNo))
            return ServiceResult<(Gender gender, Country country)>.Failure(["A person with this national number already exists"], FailureType.Conflict);

        ServiceResult<(Gender gender, Country country)> referenceDataValidation = await GetValidatedReferenceDataAsync(createPersonDto.GenderID, createPersonDto.NationalityCountryID);

        if (!referenceDataValidation.IsSuccess)
            return ServiceResult<(Gender gender, Country country)>.Failure(referenceDataValidation.Errors, referenceDataValidation.ResultType!.Value);

        Gender gender = referenceDataValidation.Data.gender;
        Country country = referenceDataValidation.Data.country;

        return ServiceResult<(Gender gender, Country country)>.Success((gender, country));
    }

    private async Task<ServiceResult<Person>> ValidateAndGetPersonForUpdateAsync(int personId, UpdatePersonDto updatePersonDto)
    {
        if (await NationalNoExistsForAnotherPersonAsync(updatePersonDto.NationalNo, personId))
            return ServiceResult<Person>.Failure(["This national number exists for another person"], FailureType.Conflict);

        Person? person = await _context.People
        .Include(p => p.Gender)
        .Include(p => p.NationalityCountry)
        .SingleOrDefaultAsync(p => p.PersonID == personId);

        if (person == null)
            return ServiceResult<Person>.Failure(["This person doesn't exist"], FailureType.NotFound);

        return ServiceResult<Person>.Success(person);
    }

    public async Task<PagedResultDto<PersonDto>> GetAllPeopleAsync(PeopleQueryParameters parameters)
    {
        IQueryable<Person> query = _context.People
        .AsNoTracking()
        .ApplySearch(parameters.SearchTerm)
        .ApplyFilters(parameters)
        .ApplySort(parameters);

        int totalItems = await query.CountAsync();

        List<PersonDto> people = await query
        .ApplyPagination(parameters)
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
            _imageService.GetImageUrl(p.ImagePath)
        ))
        .ToListAsync();

        return new PagedResultDto<PersonDto>
        {
            Items = people,
            TotalItems = totalItems,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    public async Task<Person?> GetPersonByIdWithDetailsAsync(int personId)
    {
        return await _context.People
        .Include(p => p.Gender)
        .Include(p => p.NationalityCountry)
        .SingleOrDefaultAsync(p => p.PersonID == personId);
    }

    public async Task<Person?> GetPersonByIdAsync(int personId)
    {
        return await _context.People.FindAsync(personId);
    }

    public async Task<PersonDto?> GetPersonDtoByIdAsync(int personId)
    {
        Person? person = await _context.People.Where(p => p.PersonID == personId)
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
        ServiceResult<(Gender gender, Country country)> createPersonDtoValidation = await ValidateCreatePersonAsync(createPersonDto);

        if (!createPersonDtoValidation.IsSuccess)
            return ServiceResult<PersonDto>.Failure(createPersonDtoValidation.Errors, createPersonDtoValidation.ResultType!.Value);

        Gender gender = createPersonDtoValidation.Data.gender;
        Country country = createPersonDtoValidation.Data.country;

        Person person = createPersonDto.ToEntity(gender, country);

        await _context.People.AddAsync(person);

        await _context.SaveChangesAsync();

        return ServiceResult<PersonDto>.Success(person.ToDto(_imageService.GetImageUrl(person.ImagePath)));
    }

    public async Task<ServiceResult<PersonDto>> UpdatePersonAsync(int personId, UpdatePersonDto updatePersonDto)
    {
        ServiceResult<Person> updatePersonDtoValidation = await ValidateAndGetPersonForUpdateAsync(personId, updatePersonDto);

        if (!updatePersonDtoValidation.IsSuccess)
            return ServiceResult<PersonDto>.Failure(updatePersonDtoValidation.Errors, updatePersonDtoValidation.ResultType!.Value);

        ServiceResult<(Gender gender, Country country)> referenceDataValidation = await GetValidatedReferenceDataAsync(updatePersonDto.GenderID, updatePersonDto.NationalityCountryID);

        if (!referenceDataValidation.IsSuccess)
            return ServiceResult<PersonDto>.Failure(referenceDataValidation.Errors, referenceDataValidation.ResultType!.Value);

        Person person = updatePersonDtoValidation.Data!;
        Gender updatedGender = referenceDataValidation.Data.gender;
        Country updatedCountry = referenceDataValidation.Data.country;

        person.UpdateFromDto(updatePersonDto, updatedGender, updatedCountry);

        await _context.SaveChangesAsync();

        return ServiceResult<PersonDto>.Success(person.ToDto(_imageService.GetImageUrl(person.ImagePath)));
    }

    public async Task<ServiceResult<PersonDto>> UpdatePersonImageAsync(int personId, IFormFile image)
    {
        Person? person = await GetPersonByIdWithDetailsAsync(personId);

        if (person == null)
            return ServiceResult<PersonDto>.Failure(["The requested person was not found"], FailureType.NotFound);

        ServiceResult<string> imageUploadResult = await _imageService.UploadImageAsync(image);

        if (!imageUploadResult.IsSuccess)
            return ServiceResult<PersonDto>.Failure(imageUploadResult.Errors, FailureType.ValidationError);

        string? oldImage = person.ImagePath;

        person.ImagePath = imageUploadResult.Data;

        await _context.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(oldImage))
            await _imageService.DeleteImage(oldImage);

        return ServiceResult<PersonDto>.Success(person.ToDto(_imageService.GetImageUrl(person.ImagePath)));
    }

    public async Task<bool> DeletePersonAsync(int personId)
    {
        Person? person = await _context.People
            .FindAsync(personId);

        if (person == null)
            return false;

        string? imagePath = person.ImagePath;

        _context.People.Remove(person);

        await _context.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(imagePath))
            await _imageService.DeleteImage(imagePath);

        return true;
    }

    public async Task<PersonDto?> GetPersonDtoByNationalNoAsync(string nationalNo)
    {
        Person? person = await _context.People
        .Where(p => p.NationalNo == nationalNo)
        .AsNoTracking()
        .Include(p => p.Gender)
        .Include(p => p.NationalityCountry)
        .SingleOrDefaultAsync();

        if (person == null)
            return null;

        return person.ToDto(_imageService.GetImageUrl(person.ImagePath));
    }

    public async Task<bool> PersonExistsAsync(int personId)
    {
        return await _context.People.AnyAsync(p => p.PersonID == personId);
    }
}