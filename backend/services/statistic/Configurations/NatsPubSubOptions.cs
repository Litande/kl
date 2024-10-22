namespace KL.Statistics.Configurations;

public class NatsPubSubOptions
{
    public bool Enabled { get; init; } = false;
    public string LeadsStatisticsUpdate { get; init; } = null!;
    public string AgentChangedStatus { get; init; } = null!;
}