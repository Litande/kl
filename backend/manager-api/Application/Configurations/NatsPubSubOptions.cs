namespace Plat4Me.DialClientApi.Application.Configurations;

public class NatsPubSubOptions
{
    public bool Enabled { get; init; } = false;
    public string LeadsQueueUpdate { get; init; } = null!;
    public string AgentChangedStatus { get; init; } = null!;
    public string RuleEngineRun { get; init; } = null!;
    public string LeadsStatisticsUpdate { get; init; } = null!;
    public string RatioChanged { get; init; } = null!;
    public string HangupCall { get; init; } = null!;
    public string AgentBlocked { get; init; } = null!;
    public string LeadBlocked { get; init; } = null!;
}
