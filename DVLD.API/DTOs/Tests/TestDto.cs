namespace DVLD.API.DTOs.Tests;

public class TestDto
{
    public int TestID { get; set; }
    public string TestType { get; set; } = null!;
    public string ApplicantName { get; set; } = null!;
    public string NationalNo { get; set; } = null!;
    public DateTime AppointmentTime { get; set; }
    public bool Passed { get; set; }
    public string? Notes { get; set; }
    public string CreatedByUserName { get; set; } = null!;

    public TestDto(int testID, string testType, string applicantName, string nationalNo, DateTime appointmentTime, bool passed, string? notes, string createdByUserName)
    {
        TestID = testID;
        TestType = testType;
        ApplicantName = applicantName;
        NationalNo = nationalNo;
        AppointmentTime = appointmentTime;
        Passed = passed;
        Notes = notes;
        CreatedByUserName = createdByUserName;
    }
}