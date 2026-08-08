using DVLD.API.DTOs.LicenseClasses;
using DVLD.API.Services.Interfaces;
using DVLD.DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DVLD.API.Controllers;

[ApiController]
[Route("api/license-classes")]
public class LicenseClassesController : ControllerBase
{

    private readonly ILicenseClassService _licenseClassService;

    public LicenseClassesController(ILicenseClassService licenseClassService)
    {
        _licenseClassService = licenseClassService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LicenseClassDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LicenseClassDto>>> GetAllLicenseClasses()
    {
        return Ok(await _licenseClassService.GetAllLicenseClassesAsync());
    }

    [HttpGet("{licenseClassId}")]
    [ProducesResponseType(typeof(LicenseClassDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LicenseClassDto>> GetLicenseClassById(int licenseClassId)
    {
        LicenseClassDto? licenseClass = await _licenseClassService.GetLicenseClassDtoByIdAsync(licenseClassId);

        if (licenseClass == null)
            return NotFound();

        return Ok(licenseClass);
    }
}