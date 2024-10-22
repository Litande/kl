using KL.Provider.Leads.Application.Enums;

namespace KL.Provider.Leads.Persistent.Entities;

public class DataSource
{
    public long Id { get; set; }
    public long ClientId { get; set; }
    public string Name { get; set; } = null!;
    public string ApiKey { get; set; } = null!;
    public string Endpoint { get; set; } = null!;
    public DataSourceStatusType Status { get; set; }
    public string? IframeTemplate { get; set; } = null!;
    public DateTimeOffset? MinUpdateDate { get; set; }
    public string? QueryParams { get; set; }
    public string? CallbackEndpoints { get; set; }
}