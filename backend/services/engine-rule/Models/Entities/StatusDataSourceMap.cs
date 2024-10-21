using Plat4Me.DialRuleEngine.Application.Enums;

namespace Plat4Me.DialRuleEngine.Application.Models.Entities;

public class StatusDataSourceMap
{
    public long DataSourceId { get; set; }
    public LeadStatusTypes Status { get; set; } = LeadStatusTypes.NewLead;
    public string ExternalStatusId { get; set; } = null!;
}