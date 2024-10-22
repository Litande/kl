using KL.Engine.Rule.Enums;

namespace KL.Engine.Rule.Models.Entities;

public class StatusDataSourceMap
{
    public long DataSourceId { get; set; }
    public LeadStatusTypes Status { get; set; } = LeadStatusTypes.NewLead;
    public string ExternalStatusId { get; set; } = null!;
}