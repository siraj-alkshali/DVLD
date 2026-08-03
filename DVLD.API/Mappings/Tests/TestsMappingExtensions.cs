using DVLD.API.DTOs.Tests;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Mappings.Tests;

public static class TestMappingExtensions
{
    public static TestDto ToDto(this Test test)
    {
        return new TestDto(
            test.TestID,
            test.TestAppointment.TestType.TestTypeTitle,
            $"{test.TestAppointment.LocalDrivingLicenseApplication.BaseApplication.ApplicantPerson.FirstName} {test.TestAppointment.LocalDrivingLicenseApplication.BaseApplication.ApplicantPerson.LastName}",
            test.TestAppointment.LocalDrivingLicenseApplication.BaseApplication.ApplicantPerson.NationalNo,
            test.TestAppointment.AppointmentTime,
            test.Passed,
            test.Notes,
            test.CreatedByUser.UserName
        );
    }
}