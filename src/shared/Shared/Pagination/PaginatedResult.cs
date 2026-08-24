namespace Shared.Pagination;

public record PaginatedResult<T>(
    int PageIndex,
    int PageSize,
    long Count,
    IEnumerable<T> Data
) where T : class;
