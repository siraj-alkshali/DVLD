using DVLD.API.DTOs.LicenseIssueReasons;
using DVLD.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DVLD.API.Controllers;

[ApiController]
[Route("api/license-issue-reasons")]
public class LicenseIssueReasonsController : ControllerBase
{
    private readonly ILicenseIssueReasonService _licenseIssueReasonService;

    public LicenseIssueReasonsController(ILicenseIssueReasonService licenseIssueReasonService)
    {
        _licenseIssueReasonService = licenseIssueReasonService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LicenseIssueReasonDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LicenseIssueReasonDto>>> GetAllTestTypes()
    {
        return Ok(await _licenseIssueReasonService.GetAllLicenseIssueReasonsAsync());
    }

    [HttpGet("{id}", Name = "GetLicenseIssueReasonById")]
    [ProducesResponseType(typeof(LicenseIssueReasonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LicenseIssueReasonDto>> GetLicenseIssueReasonById(int id)
    {
        LicenseIssueReasonDto? licenseIssueReason = await _licenseIssueReasonService.GetLicenseIssueReasonById(id);

        if (licenseIssueReason == null)
            return NotFound();

        return Ok(licenseIssueReason);
    }
}