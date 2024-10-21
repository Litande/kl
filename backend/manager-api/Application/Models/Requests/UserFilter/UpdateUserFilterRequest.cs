namespace Plat4Me.DialClientApi.Application.Models.Requests.UserFilter;

public class UpdateUserFilterRequest
{
    public long Id { get; set; }
    public string FilterName { get; set; } = null!;
    public LeadSearchGroupParams[] Filter { get; set; } =  Array.Empty<LeadSearchGroupParams>();
}