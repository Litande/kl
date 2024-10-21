namespace Plat4Me.DialRuleEngine.Application.App;

public class PubSubjects
{
    public string LeadsQueueUpdate { get; set; } = null!;
    public string RuleEngineRun { get; set; } = null!;
    public string LeadsStatisticsUpdate { get; set; } = null!;
    public string LeadFeedbackProcessed { get; set; } = null!;
}