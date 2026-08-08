using DVLD.API.Services.Interfaces;
using DVLD.DataAccess.Data;
using DVLD.API.DTOs.DetainedLicenses;
using Microsoft.EntityFrameworkCore;
using DVLD.DataAccess.Entities;
using DVLD.API.Common.Results;

namespace DVLD.API.Services;

public class DetainedLicenseService : IDetainedLicenseService
{
    private readonly DVLDContext _context;
    private readonly ILicenseService _licenseService;
    private readonly ICurrentUserService _currentUserService;

    public DetainedLicenseService(DVLDContext context, ILicenseService licenseService, ICurrentUserService currentUserService)
    {
        _context = context;
        _licenseService = licenseService;
        _currentUserService = currentUserService;
    }

    private async Task<ServiceResult<License>> ValidateAndGetLicenseForDetentionAsync(DetainLicenseDto detainLicenseDto)
    {
        if (detainLicenseDto.FineFees <= 0)
            return ServiceResult<License>.Failure(["Fine fees must be greater than zero"], FailureType.ValidationError);

        License? license = await _licenseService.GetLicenseByIdForDetentionAsync(detainLicenseDto.LicenseID);

        if (license == null)
            return ServiceResult<License>.Failure(["This license does not exist"], FailureType.NotFound);

        if (!license.IsActive)
            return ServiceResult<License>.Failure(["This license is inactive"], FailureType.Conflict);

        if (license.Detentions.Any(d => d.ReleaseDate == null))
            return ServiceResult<License>.Failure(["This license is already detained"], FailureType.Conflict);

        return ServiceResult<License>.Success(license);
    }

    private DetainedLicense BuildDetainedLicenseEntity(License license, decimal fineFees)
    {
        return new DetainedLicense
        {
            LicenseID = license.LicenseID,
            DetainDate = DateOnly.FromDateTime(DateTime.Now),
            FineFees = fineFees,
            CreatedByUserID = _currentUserService.UserID,
        };
    }

    public async Task<DetainedLicenseDto?> GetDetainedLicenseDtoByIdAsync(int detainId)
    {
        return await _context.DetainedLicenses
        .AsNoTracking()
        .Where(d => d.DetainID == detainId)
        .Select(d => new DetainedLicenseDto(
            d.DetainID,
            $"{d.License.Driver.Person.FirstName} {d.License.Driver.Person.LastName}",
            d.License.Driver.Person.NationalNo,
            d.License.Driver.Person.Phone,
            d.FineFees,
            d.DetainDate,
            d.ReleaseDate,
            d.ReleasedByUser == null ? null : d.ReleasedByUser.UserName
        )).SingleOrDefaultAsync();
    }

    public async Task<ServiceResult<DetainedLicenseDto>> DetainLicenseAsync(DetainLicenseDto detainLicenseDto)
    {
        ServiceResult<License> validationForLicenseDetention = await ValidateAndGetLicenseForDetentionAsync(detainLicenseDto);

        if (!validationForLicenseDetention.IsSuccess)
            return ServiceResult<DetainedLicenseDto>.Failure(validationForLicenseDetention.Errors, validationForLicenseDetention.ResultType!.Value);

        License license = validationForLicenseDetention.Data!;
        Driver driver = license.Driver;
        Person person = driver.Person;

        DetainedLicense detainedLicense = BuildDetainedLicenseEntity(license, detainLicenseDto.FineFees);

        await _context.DetainedLicenses.AddAsync(detainedLicense);

        license.IsActive = false;

        await _context.SaveChangesAsync();

        DetainedLicenseDto detainedLicenseDto = new DetainedLicenseDto(
            detainedLicense.DetainID,
            $"{person.FirstName} {person.LastName}",
            person.NationalNo,
            person.Phone,
            detainedLicense.FineFees,
            detainedLicense.DetainDate,
            detainedLicense.ReleaseDate,
            null
        );

        return ServiceResult<DetainedLicenseDto>.Success(detainedLicenseDto);
    }
}