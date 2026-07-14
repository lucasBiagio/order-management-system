namespace OrderManagement.Application.Common.Responses;

public sealed class PagedResponse<T>
{
    public IReadOnlyCollection<T> Items { get; init; } = [];

    public int Page { get; init; }

    public int PageSize { get; init; }

    public int TotalItems { get; init; }

    public int TotalPages =>
        (int)Math.Ceiling((double)TotalItems / PageSize);
}