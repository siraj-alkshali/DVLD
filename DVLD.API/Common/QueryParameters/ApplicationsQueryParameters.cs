namespace DVLD.API.Common.QueryParameters;

public class ApplicationsQueryParameters : SearchParameters
{
    public int? ApplicationTypeID { get; set; }
    public int? ApplicationStatusID { get; set; }
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
}