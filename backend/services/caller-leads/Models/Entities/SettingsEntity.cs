using KL.Caller.Leads.Enums;

namespace KL.Caller.Leads.Models.Entities;

public class SettingsEntity
{
    public long Id { get; set; }
    public string Value { get; set; } = null!;
    public long ClientId { get; set; }
    public SettingTypes Type { get; set; }
}