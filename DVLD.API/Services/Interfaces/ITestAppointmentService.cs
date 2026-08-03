using DVLD.API.Common.Results;
using DVLD.API.DTOs.TestAppointments;

namespace DVLD.API.Services.Interfaces;

public interface ITestAppointmentService
{
    Task<ServiceResult<TestAppointmentDto>> CreateTestAppointmentAsync(CreateTestAppointmentDto dto);
}