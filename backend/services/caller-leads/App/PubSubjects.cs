namespace KL.Caller.Leads.App;

public class PubSubjects
{
    /// <summary>
    /// Use for forward agent changed status
    /// </summary>
    public string AgentChangedStatus { get; set; } = null!;

    /// <summary>
    /// Use for answer to the message AgentNotAnswered
    /// </summary>
    public string AgentReplaceResult { get; set; } = null!;

    /// <summary>
    /// Use for send data to try do a call
    /// </summary>
    public string TryCallToLead { get; set; } = null!;
    public string CallInitiated{ get; set; } = null!;

    /// <summary>
    /// Use for send lead to process behaviour rules
    /// </summary>
    public string LeadFeedbackCallFailed { get; set; } = null!;
    public string LeadFeedbackFilled { get; set; } = null!;

    public string BridgeRegRequest { get; set; } = null!;

    /// <summary>
    /// User for notify about queue update
    /// </summary>
    public string LeadsQueueUpdate { get; set; } = null!;

    /// <summary>
    /// Use for notify about lead statistic updated
    /// </summary>
    public string LeadsStatisticsUpdate { get; set; } = null!;

    /// <summary>
    /// Use to notify a new CDR is updated
    /// </summary>
    public string CdrUpdated { get; set; } = null!;

    /// <summary>
    /// Use to notify a new CDR is created
    /// </summary>
    public string CdrInserted { get; set; } = null!;
}
