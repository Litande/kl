using Plat4Me.DialClientApi.Application.Enums;

namespace Plat4Me.DialClientApi.Persistent.Entities;

public class SettingsEntity
{
    public long Id { get; set; }
    public string Value { get; set; } = null!;
    public long ClientId { get; set; }
    public SettingTypes Type { get; set; }
}