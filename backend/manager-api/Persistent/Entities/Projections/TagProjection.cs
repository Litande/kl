using Plat4Me.DialClientApi.Application.Enums;

namespace Plat4Me.DialClientApi.Persistent.Entities.Projections;

public class TagProjection
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public int Value { get; set; }
    public TagStatusTypes Status { get; set; }
}