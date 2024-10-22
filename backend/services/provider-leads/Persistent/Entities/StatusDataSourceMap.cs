using KL.Provider.Leads.Application.Enums;

namespace KL.Provider.Leads.Persistent.Entities;

public class StatusDataSourceMap
{
    public long Id { get; set; }
    public long DataSourceId { get; set; }
    public LeadStatusTypes Status { get; set; }
    public string ExternalStatusId { get; set; } = null!;
}