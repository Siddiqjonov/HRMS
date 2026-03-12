using Microsoft.EntityFrameworkCore;

namespace HrManager.Application.UseCases.Employees.GetEmployeesWithPagination;

public class PaginatedList<T>
{
    private PaginatedList(List<T> items, int pageNumber, int pageSize, int totalCount)
    {
        Items = items;
        Page = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }

    public IReadOnlyList<T> Items { get; }

    public int Page { get; }

    public int PageSize { get; }

    public int TotalCount { get; }

    public int TotalPages { get; }

    public bool HasNextPage => Page < TotalPages;

    public bool HasPreviousPage => Page > 1;

    public static async Task<PaginatedList<T>> CreateAsync(
        IQueryable<T> query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new (items, pageNumber, pageSize, totalCount);
    }
}