using Plat4Me.DialClientApi.Application.Enums;
using Plat4Me.DialClientApi.Persistent.Entities.Projections;

namespace Plat4Me.DialClientApi.Application.Models.Requests;

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

public class AgentTagsPaginationRequest : PaginationRequest
{
    protected override string DefaultSortProperty => nameof(AgentTagsProjection.Score);
}

public class LeadSearchPaginationRequest : PaginationRequest
{
    protected override string DefaultSortProperty => nameof(LeadSearchProjection.LeadId);
}

public class CallSearchPaginationRequest : PaginationRequest
{
    protected override string DefaultSortProperty => nameof(CDRProjection.LeadId);
}

public class LeadBlacklistPaginationRequest : PaginationRequest
{
    protected override string DefaultSortProperty => nameof(LeadBlacklistProjection.Id);
}

public class LeadQueuePaginationRequest : PaginationRequest
{
    protected override string DefaultSortProperty => null;
}