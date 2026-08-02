namespace DVLD.API.Common.QueryParameters;

public class SearchParameters : PaginationParameters
{
    public string? SearchTerm { get; set; }
    public string SortDirection { get; set; } = "asc";
    public string? SortBy { get; set; }
}