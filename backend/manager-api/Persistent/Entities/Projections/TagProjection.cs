using KL.Manager.API.Application.Enums;

namespace KL.Manager.API.Persistent.Entities.Projections;

public class TagProjection
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public int Value { get; set; }
    public TagStatusTypes Status { get; set; }
}