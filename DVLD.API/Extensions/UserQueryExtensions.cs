using DVLD.API.Common.QueryParameters;
using DVLD.DataAccess.Entities;

namespace DVLD.API.Extensions;

public static class UserQueryExtensions
{
    public static IQueryable<User> ApplySearch(this IQueryable<User> query, string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return query;

        searchTerm = searchTerm.Trim();

        return query.Where(u => u.UserName == searchTerm
        || u.Person.FirstName.Contains(searchTerm)
        || u.Person.LastName.Contains(searchTerm));
    }

    public static IQueryable<User> ApplyFilter(this IQueryable<User> query, UsersQueryParameters? parameters)
    {
        if (parameters == null)
            return query;

        if (parameters.IsActive.HasValue)
            query = query.Where(u => u.IsActive == parameters.IsActive);

        if (parameters.RoleID.HasValue)
            query = query.Where(u => u.RoleID == parameters.RoleID);

        return query;
    }

    public static IQueryable<User> ApplySort(this IQueryable<User> query, UsersQueryParameters parameters)
    {
        bool descending = string.Equals(parameters.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return parameters.SortBy?.ToLower() switch
        {
            "firstname" =>
                descending
                    ? query.OrderByDescending(u => u.Person.FirstName)
                        .ThenByDescending(u => u.UserID)

                    : query.OrderBy(u => u.Person.FirstName)
                        .ThenByDescending(u => u.UserID),

            "lastname" =>
                descending
                    ? query.OrderByDescending(u => u.Person.LastName)
                        .ThenByDescending(u => u.UserID)

                    : query.OrderBy(u => u.Person.LastName)
                        .ThenByDescending(u => u.UserID),

            _ =>
                query.OrderBy(u => u.UserID)
        };
    }
}