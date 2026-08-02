using DVLD.API.Common.QueryParameters;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Extensions;

public static class PersonQueryExtensions
{
    public static IQueryable<Person> ApplySearch(this IQueryable<Person> query, string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return query;

        searchTerm = searchTerm.Trim();

        return query.Where(p => p.NationalNo == searchTerm
        || p.FirstName.Contains(searchTerm)
        || p.SecondName.Contains(searchTerm)
        || p.LastName.Contains(searchTerm)
        || p.Phone.Contains(searchTerm));
    }

    public static IQueryable<Person> ApplyFilters(this IQueryable<Person> query, PeopleQueryParameters? parameters)
    {
        if (parameters == null)
            return query;

        if (parameters.GenderID.HasValue)
            query = query.Where(p => p.GenderID == parameters.GenderID.Value);

        if (parameters.NationalityCountryID.HasValue)
            query = query.Where(p => p.NationalityCountryID == parameters.NationalityCountryID.Value);

        return query;
    }

    public static IQueryable<Person> ApplySort(this IQueryable<Person> query, PeopleQueryParameters parameters)
    {
        bool descending = string.Equals(parameters.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return parameters.SortBy?.ToLower() switch
        {
            "firstname" =>
                descending
                    ? query.OrderByDescending(p => p.FirstName)
                    : query.OrderBy(p => p.FirstName),


            "lastname" =>
                descending
                    ? query.OrderByDescending(p => p.LastName)
                    : query.OrderBy(p => p.LastName),

            _ =>
                query.OrderBy(p => p.PersonID)
        };
    }

}