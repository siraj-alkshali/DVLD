namespace DVLD.API.DTOs.Tests;

public class TestListItemDto
{
    public int TestID { get; set; }
    public string TestType { get; set; }
    public DateTime AppointmentTime { get; set; }
    public bool Passed { get; set; }

    public TestListItemDto(int testId, string testType, DateTime appointmentTime, bool passed)
    {
        TestID = testId;
        TestType = testType;
        AppointmentTime = appointmentTime;
        Passed = passed;
    }
}