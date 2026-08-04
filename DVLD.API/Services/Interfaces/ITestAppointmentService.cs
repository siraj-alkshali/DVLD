using DVLD.API.Common.Results;
using DVLD.API.DTOs.TestAppointments;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Services.Interfaces;

public interface ITestAppointmentService
{
    Task<ServiceResult<TestAppointmentDto>> CreateTestAppointmentAsync(CreateTestAppointmentDto dto);
    Task<bool> RetakeTestAlreadyBooked(int localAppId, int testTypeId);
}