namespace DVLD.API.DTOs.Tests;

public class CreateTestDto
{
    public int TestAppointmentID { get; set; }
    public bool Passed { get; set; }
    public string? Notes { get; set; }
}