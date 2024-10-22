namespace KL.Agent.API.Application.Configurations;

public class NatsPubSubOptions
{
    public bool Enabled { get; init; } = false;
    public string InviteAgent { get; init; } = null!;
    public string AgentChangedStatus { get; init; } = null!;
    public string LeadFeedbackFilled { get; init; } = null!;
    public string ManualCall { get; init; } = null!;
    public string CallAgain { get; init; } = null!;
    public string CallFinished { get; init; } = null!;
    public string CallFailed { get; init; } = null!;
    public string AgentBlocked { get; init; } = null!;
    public string EnqueueAgentForCall { get; set; } = null!;
    public string DequeueAgentForCall { get; set; } = null!;
    public string CallInitiated{ get; set; } = null!;
    public string AgentAnswered { get; set; } = null!;
    public string LeadAnswered { get; set; } = null!;
    public string DroppedAgent { get; set; } = null!;
    public string AgentReplaceResult { get; set; } = null!;
}
