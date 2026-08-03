namespace DVLD.API.DTOs.TestAppointments;

public class CreateTestAppointmentDto
{
    public int TestTypeID { get; set; }
    public int LocalDrivingLicenseApplicationID { get; set; }
    public DateTime AppointmentTime { get; set; }
}