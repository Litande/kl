namespace KL.Caller.Leads.App;

public class SubSubjects
{

    public string EnqueueAgentForCall { get; set; } = null!;
    public string DequeueAgentForCall { get; set; } = null!;

    public string CallFailed { get; set; } = null!;
    public string CallFinished { get; set; } = null!;
    public string CallFinishedRecords { get; set; } = null!;
    public string AgentAnswered { get; set; } = null!;
    public string LeadAnswered { get; set; } = null!;

    /// <summary>
    /// Incoming message where handler try replace agent
    /// </summary>
    public string AgentNotAnswered { get; set; } = null!;

    public string LeadFeedbackFilled { get; set; } = null!;
    public string MixedRecordReady { get; set; } = null!;

    public string ManualCall { get; set; } = null!;
    public string CallAgain { get; init; } = null!;

    public string BridgeRun { get; set; } = null!;
    public string DroppedAgent { get; set; } = null!;
}
