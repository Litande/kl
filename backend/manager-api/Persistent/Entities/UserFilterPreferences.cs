namespace Plat4Me.DialClientApi.Persistent.Entities;

public class UserFilterPreferences
{
    public long Id { get; set; }
    public string FilterName { get; set; } = null!;
    public string Filter = null!;
    public long CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;
}