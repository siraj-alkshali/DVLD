namespace DVLD.API.Common.QueryParameters;

public class PeopleQueryParameters : SearchParameters
{
    public int? GenderID { get; set; }
    public int? NationalityCountryID { get; set; }
}