using DVLD.API.DTOs.ApplicationTypes;
using DVLD.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DVLD.API.Controllers;

[ApiController]
[Route("api/application-types")]
public class ApplicationTypesController : ControllerBase
{
    private readonly IApplicationTypeService _applicationTypeService;

    public ApplicationTypesController(IApplicationTypeService applicationTypeService)
    {
        _applicationTypeService = applicationTypeService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ApplicationTypeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ApplicationTypeDto>>> GetAllApplicationTypes()
    {
        return Ok(await _applicationTypeService.GetAllApplicationTypesAsync());
    }

    [HttpGet("{applicationTypeId}", Name = "GetApplicationTypeById")]
    [ProducesResponseType(typeof(ApplicationTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationTypeDto>> GetApplicationTypeById(int applicationTypeId)
    {
        ApplicationTypeDto? applicationType = await _applicationTypeService.GetApplicationTypeDtoByIdAsync(applicationTypeId);

        if (applicationType == null)
            return NotFound();

        return Ok(applicationType);
    }
}