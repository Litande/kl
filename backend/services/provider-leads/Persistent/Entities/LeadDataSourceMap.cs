using Plat4Me.DialLeadProvider.Application.Enums;

namespace Plat4Me.DialLeadProvider.Persistent.Entities;

public class LeadDataSourceMap
{
    public long Id { get; set; }
    public long DataSourceId { get; set; }
    public LeadDataSource DestinationProperty { get; set; }
    public string SourceProperty { get; set; } = null!;
}