using Plat4Me.DialLeadProvider.Application.Enums;

namespace Plat4Me.DialLeadProvider.Persistent.Entities;

public class SettingsEntity
{
    public long Id { get; set; }
    public string Value { get; set; } = null!;
    public long ClientId { get; set; }
    public SettingTypes Type { get; set; }
}