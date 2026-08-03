using DVLD.API.DTOs.TestAppointments;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Mappings.TestAppointments;

public static class TestAppointmentMappingExtensions
{
    public static TestAppointmentDto ToDto(this TestAppointment testAppointment)
    {
        return new TestAppointmentDto(
            testAppointment.TestAppointmentID,
            testAppointment.TestType.TestTypeTitle,
            $"{testAppointment.LocalDrivingLicenseApplication.BaseApplication.ApplicantPerson.FirstName} {testAppointment.LocalDrivingLicenseApplication.BaseApplication.ApplicantPerson.LastName}",
            testAppointment.LocalDrivingLicenseApplication.BaseApplication.ApplicantPerson.NationalNo,
            testAppointment.AppointmentTime,
            testAppointment.CreatedByUser.UserName
        );
    }
}