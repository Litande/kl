namespace Plat4Me.DialClientApi.Persistent.Entities;

public class Team
{
    public long Id { get; set; }
    public long ClientId { get; set; }
    public string Name { get; set; } = null!;
    public virtual ICollection<User> Agents { get; set; } = new HashSet<User>();
}