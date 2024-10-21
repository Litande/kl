namespace Plat4Me.DialClientApi.Application.Models;

public class LeadSearchGroupParams
{
    public string Name { get; set; } = null!;
    public LeadAdditionalFields[] Fields { get; set; } = Array.Empty<LeadAdditionalFields>();
}

public class LeadAdditionalFields
{
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public bool IsActive { get; set; }
}