using Plat4Me.DialAgentApi.Application.Enums;
using Plat4Me.DialAgentApi.Persistent.Entities.Projections;

namespace Plat4Me.DialAgentApi.Application.Models.Requests;

public class PaginationRequest
{
    protected virtual int DefaultPage => 1;
    protected virtual int DefaultPageSize => 10;
    protected virtual string DefaultSortProperty => "Id";
    protected virtual SortDirectionTypes DefaultSortDirection => SortDirectionTypes.Desc;

    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? SortBy { get; set; }
    public SortDirectionTypes? SortDirection { get; set; }

    public PaginationRequest Create(PaginationRequest? pagination)
    {
        return new PaginationRequest
        {
            Page = pagination?.Page is null || pagination.Page < 1 ? DefaultPage : pagination.Page,
            PageSize = pagination?.PageSize is null || pagination.PageSize < 1 ? DefaultPageSize : pagination.PageSize,
            SortBy = pagination?.SortBy ?? DefaultSortProperty,
            SortDirection = pagination?.SortDirection ?? DefaultSortDirection
        };
    }
}

public class FutureCallBackPaginationRequest : PaginationRequest
{
    protected override int DefaultPageSize => 10;
    protected override string DefaultSortProperty => nameof(FutureCallBackProjection.NextCallAt);
}

public class CDRHistoryPaginationRequest : PaginationRequest
{
    protected override int DefaultPageSize => 15;
    protected override string DefaultSortProperty => nameof(CDRHistoryProjection.LastActivity);
}

public class CDRAgentHistoryPaginationRequest : PaginationRequest
{
    protected override string DefaultSortProperty => nameof(CDRAgentHistoryProjection.Date);
}

public class LeadCommentPaginationRequest : PaginationRequest
{
    protected override SortDirectionTypes DefaultSortDirection => SortDirectionTypes.Desc;
    protected override string DefaultSortProperty => nameof(LeadCommentProjection.CreatedAt);
}