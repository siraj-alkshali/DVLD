using DVLD.API.Common.QueryParameters;

namespace DVLD.API.Extensions;

public static class IQueryableExtensions
{
    public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, PaginationParameters parameters)
    {
        return query.Skip(parameters.Skip).Take(parameters.PageSize);
    }
}