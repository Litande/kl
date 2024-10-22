namespace KL.Manager.API.Persistent.Entities.Projections;

public class AgentTagsProjection
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? TeamName { get; set; }
    public int Score { get; set; }
    public IEnumerable<TagProjection> Tags { get; set; } = Enumerable.Empty<TagProjection>();
}