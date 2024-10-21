using Plat4Me.DialLeadProvider.Application.Enums;

namespace Plat4Me.DialLeadProvider.Persistent.Entities;

public class StatusDataSourceMap
{
    public long Id { get; set; }
    public long DataSourceId { get; set; }
    public LeadStatusTypes Status { get; set; }
    public string ExternalStatusId { get; set; } = null!;
}