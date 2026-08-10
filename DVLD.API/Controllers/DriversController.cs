using DVLD.API.Common.QueryParameters;
using DVLD.API.DTOs;
using DVLD.API.DTOs.Common;
using DVLD.API.DTOs.Licenses;
using DVLD.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DVLD.API.Controllers;

[ApiController]
[Route("api/drivers")]
public class DriversController : ControllerBase
{
    private readonly IDriverService _driverService;
    private readonly ILicenseService _licenseService;

    public DriversController(IDriverService driverService, ILicenseService licenseService)
    {
        _driverService = driverService;
        _licenseService = licenseService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<DriverDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<DriverDto>>> GetAllDrivers([FromQuery] DriversQueryParameters parameters)
    {
        return Ok(await _driverService.GetAllDriversAsync(parameters));
    }

    [HttpGet("{driverId}/licenses")]
    [ProducesResponseType(typeof(List<LicenseListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<LicenseListItemDto>>> GetAllLicensesForDriver(int driverId)
    {
        return Ok(await _licenseService.GetAllLicensesForDriverAsync(driverId));
    }
}