using Plat4Me.DialClientApi.Application.Enums;

namespace Plat4Me.DialClientApi.Persistent.Entities;

public class DataSource
{
    public long Id { get; set; }
    public long ClientId { get; set; }
    public DataSourceTypes DataSourceType { get; set; } = DataSourceTypes.Lead;
    public string Name { get; set; } = null!;
    public string ApiKey { get; set; } = null!;
    public string Endpoint { get; set; } = null!;
    public DataSourceStatusTypes Status { get; set; }
    public string? IframeTemplate { get; set; }
    public string? Metadata { get; set; }
    public DateTimeOffset? MinUpdateDate { get; set; } = null!;
}
