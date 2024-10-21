namespace Plat4Me.DialAgentApi.Persistent.Entities;

public class DataSource
{
    public long Id { get; set; }
    public long ClientId { get; set; }
    public string Name { get; set; } = null!;
    public string? IframeTemplate { get; set; }
}
