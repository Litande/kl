namespace KL.Caller.Leads.App;

public class LeadQueueClientOptions
{
    public string BaseUrl { get; set; } = null!;
    public string GetNextEndpoint { get; set; } = null!;
    public string RunBehaviourRulesEndpoint { get; set; } = null!;
}