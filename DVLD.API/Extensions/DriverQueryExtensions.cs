using DVLD.API.Common.QueryParameters;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Extensions;

public static class DriverQueryExtensions
{
    public static IQueryable<Driver> ApplySearch(this IQueryable<Driver> query, string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return query;

        searchTerm = searchTerm.Trim().ToLower();

        return query.Where(driver => driver.Person.FirstName.Contains(searchTerm)
        || driver.Person.LastName.Contains(searchTerm)
        || driver.Person.NationalNo == searchTerm
        || driver.Person.Phone == searchTerm);
    }

    public static IQueryable<Driver> ApplySort(this IQueryable<Driver> query, DriversQueryParameters parameters)
    {
        bool descending = string.Equals(parameters.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return parameters.SortBy?.ToLower() switch
        {
            "firstname" =>
                descending
                    ? query.OrderByDescending(d => d.Person.FirstName)
                        .ThenByDescending(d => d.DriverID)

                    : query.OrderBy(d => d.Person.FirstName)
                        .ThenBy(d => d.DriverID),

            "lastname" =>
                descending
                    ? query.OrderByDescending(d => d.Person.LastName)
                        .ThenByDescending(d => d.DriverID)

                    : query.OrderBy(d => d.Person.LastName)
                        .ThenBy(d => d.DriverID),

            _ =>
                query.OrderBy(d => d.DriverID)
        };
    }
}