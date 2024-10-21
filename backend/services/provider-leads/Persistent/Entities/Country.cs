namespace Plat4Me.DialLeadProvider.Persistent.Entities;

public class Country
{
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string? CountryCode3 { get; set; }
    public string DialCode { get; set; } = null!;
}