namespace Plat4Me.DialClientApi.Application.Models.Responses;

public class PaginatedResponse<T>
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public IEnumerable<T> Items { get; set; } = new List<T>();
}