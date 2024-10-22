namespace KL.Manager.API.Persistent.Entities.Projections;

public class StatusRuleProjection
{
    public string Status { get; set; } = null!;
    public IEnumerable<string> AvailableStatuses { get; set; } = new HashSet<string>();
}
