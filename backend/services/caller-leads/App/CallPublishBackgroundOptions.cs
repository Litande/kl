namespace KL.Caller.Leads.App;

public class CallPublishBackgroundOptions
{
    public int WaitingPeriod { get; set; } = 500;//ms
    public int IterationDelay { get; set; } = 50;//ms
    public int MessagesPerIteration { get; set; } = 1;
}
