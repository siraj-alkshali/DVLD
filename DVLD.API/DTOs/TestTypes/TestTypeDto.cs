namespace DVLD.API.DTOs.TestTypes;

public class TestTypeDto
{
    public TestTypeDto(int testTypeId, string testTypeTitle, string testTypeDescription, decimal testTypeFees)
    {
        TestTypeID = testTypeId;
        TestTypeTitle = testTypeTitle;
        TestTypeDescription = testTypeDescription;
        TestTypeFees = testTypeFees;
    }

    public int TestTypeID { get; set; }
    public string TestTypeTitle { get; set; } = null!;
    public string TestTypeDescription { get; set; } = null!;
    public decimal TestTypeFees { get; set; }
}