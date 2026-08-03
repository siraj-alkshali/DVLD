namespace DVLD.API.DTOs.TestAppointments;

public class TestAppointmentDto
{
    public int TestAppointmentID { get; set; }
    public string TestType { get; set; }
    public string ApplicantName { get; set; }
    public string NationalNo { get; set; }
    public DateTime AppointmentTime { get; set; }
    public string CreatedByUserName { get; set; }

    public TestAppointmentDto(int testAppointmentID, string testType, string applicantName, string nationalNo, DateTime appointmentTime, string createdByUserName)
    {
        TestAppointmentID = testAppointmentID;
        TestType = testType;
        ApplicantName = applicantName;
        NationalNo = nationalNo;
        AppointmentTime = appointmentTime;
        CreatedByUserName = createdByUserName;
    }

}