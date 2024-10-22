namespace KL.Manager.API.Application.Models.Responses.UserFilter;

public class UserFilterResponse
{
    public long Id { get; set; }
    public string FilterName { get; set; } = null!;
    public LeadSearchGroupParams[] Filter { get; set; } = Array.Empty<LeadSearchGroupParams>();
}