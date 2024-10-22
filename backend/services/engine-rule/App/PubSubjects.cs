namespace KL.Engine.Rule.App;

public class PubSubjects
{
    public string LeadsQueueUpdate { get; set; } = null!;
    public string RuleEngineRun { get; set; } = null!;
    public string LeadsStatisticsUpdate { get; set; } = null!;
    public string LeadFeedbackProcessed { get; set; } = null!;
}