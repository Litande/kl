using KL.Provider.Leads.Application.Enums;

namespace KL.Provider.Leads.Persistent.Entities;

public class LeadDataSourceMap
{
    public long Id { get; set; }
    public long DataSourceId { get; set; }
    public LeadDataSource DestinationProperty { get; set; }
    public string SourceProperty { get; set; } = null!;
}