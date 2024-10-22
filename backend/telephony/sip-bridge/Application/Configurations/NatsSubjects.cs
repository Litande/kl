namespace KL.SIP.Bridge.Application.Configurations;

public class NatsSubjects
{
    public string BridgeRegRequest { get; set; } = null!;
    public string BridgeRun { get; set; } = null!;
    public string TryCallToLead { get; set; } = null!;
    public string CallFailed { get; set; } = null!;
    public string CallFinished { get; set; } = null!;
    public string CallFinishedRecords { get; set; } = null!;
    public string InviteAgent { get; set; } = null!;
    public string AgentAnswered { get; set; } = null!;
    public string LeadAnswered { get; set; } = null!;
    public string AgentNotAnswered { get; set; } = null!;
    public string DroppedAgent { get; set; } = null!;
    public string AgentReplaceResult { get; set; } = null!;
    public string HangupCall { get; set; } = null!;
}
