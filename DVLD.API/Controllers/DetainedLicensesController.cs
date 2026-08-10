using DVLD.API.Common.QueryParameters;
using DVLD.API.Common.Results;
using DVLD.API.DTOs.Common;
using DVLD.API.DTOs.DetainedLicenses;
using DVLD.API.Extensions;
using DVLD.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DVLD.API.Controllers;

[ApiController]
[Route("api/detained-licenses")]
public class DetainedLicensesController : ControllerBase
{
    private readonly IDetainedLicenseService _detainedLicenseService;

    public DetainedLicensesController(IDetainedLicenseService detainedLicenseService)
    {
        _detainedLicenseService = detainedLicenseService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<DetainedLicenseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<DetainedLicenseDto>>> GetAllDetainedLicenses([FromQuery] DetainedLicensesQueryParameters parameters)
    {
        return Ok(await _detainedLicenseService.GetAllDetainedLicensesAsync(parameters));
    }

    [HttpGet("{detainedLicenseId}", Name = "GetDetainedLicenseById")]
    [ProducesResponseType(typeof(DetainedLicenseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DetainedLicenseDto>> GetDetainedLicenseById(int detainedLicenseId)
    {
        DetainedLicenseDto? detainedLicense = await _detainedLicenseService.GetDetainedLicenseDtoByIdAsync(detainedLicenseId);

        if (detainedLicense == null)
            return NotFound();

        return Ok(detainedLicense);
    }

    [HttpPost]
    [ProducesResponseType(typeof(DetainedLicenseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<DetainedLicenseDto>> DetainLicense(DetainLicenseDto detainLicenseDto)
    {
        ServiceResult<DetainedLicenseDto> result = await _detainedLicenseService.DetainLicenseAsync(detainLicenseDto);

        if (!result.IsSuccess)
            return this.ToActionResult(result);

        return CreatedAtRoute("GetDetainedLicenseById", new { detainedLicenseId = result.Data!.DetainID }, result.Data);
    }
}