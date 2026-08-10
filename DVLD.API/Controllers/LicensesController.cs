using DVLD.API.DTOs.Licenses;
using DVLD.API.Services.Interfaces;
using DVLD.API.Common.Results;
using DVLD.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using DVLD.API.Common.QueryParameters;
using DVLD.API.DTOs.Common;

namespace DVLD.API.Controllers;

[ApiController]
[Route("api/licenses")]
public class LicensesController : ControllerBase
{
    private readonly ILicenseService _licenseService;

    public LicensesController(ILicenseService licenseService)
    {
        _licenseService = licenseService;
    }

    [HttpGet("{licenseId}", Name = "GetLicenseById")]
    [ProducesResponseType(typeof(LicenseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LicenseDto>> GetLicenseById(int licenseId)
    {
        LicenseDto? license = await _licenseService.GetLicenseDtoByIdAsync(licenseId);

        if (license == null)
            return NotFound();

        return Ok(license);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<LicenseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<LicenseDto>>> GetAllLicenses([FromQuery] LicenseQueryParameters parameters)
    {
        return Ok(await _licenseService.GetAllLicensesAsync(parameters));
    }

    [HttpPost]
    [ProducesResponseType(typeof(LicenseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LicenseDto>> CreateLicense(CreateLicenseDto createLicenseDto)
    {
        ServiceResult<LicenseDto> result = await _licenseService.CreateNewLicenseAsync(createLicenseDto);

        if (!result.IsSuccess)
            return this.ToActionResult(result);

        return CreatedAtRoute("GetLicenseById", new { licenseId = result.Data!.LicenseID }, result.Data);
    }
}