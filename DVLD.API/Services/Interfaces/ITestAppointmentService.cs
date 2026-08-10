using DVLD.API.Common.Results;
using DVLD.API.DTOs.TestAppointments;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Services.Interfaces;

public interface ITestAppointmentService
{
    Task<ServiceResult<TestAppointmentDto>> CreateTestAppointmentAsync(CreateTestAppointmentDto createTestAppointmentDto);
    Task<TestAppointmentDto?> GetTestAppointmentDtoByIdAsync(int testAppointmentId);
    Task<bool> RetakeTestAlreadyBookedAsync(int localAppId, int testTypeId);
}