using DVLD.API.DTOs;
using DVLD.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DVLD.API.Controllers;

[ApiController]
[Route("api/international-licenses")]
public class InternationalLicensesController : ControllerBase
{
    private readonly IInternationalLicenseService _internationalLicenseService;

    public InternationalLicensesController(IInternationalLicenseService internationalLicenseService)
    {
        _internationalLicenseService = internationalLicenseService;
    }

    [HttpGet("{internationalLicenseId}", Name = "GetInternationalLicenseById")]
    [ProducesResponseType(typeof(InternationalLicenseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InternationalLicenseDto>> GetInternationalLicenseById(int internationalLicenseId)
    {
        InternationalLicenseDto? internationalLicense = await _internationalLicenseService.GetInternationalLicenseDtoByIdAsync(internationalLicenseId);

        if (internationalLicense == null)
            return NotFound();

        return Ok(internationalLicense);
    }
}