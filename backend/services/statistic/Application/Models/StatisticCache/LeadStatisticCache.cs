namespace KL.Statistics.Application.Models.StatisticCache;

public class LeadStatisticCache
{
    public long Amount { get; set; }
    public string? Country { get; set; }
    public List<LeadCache> Leads { get; set; }
    public TimeSpan MaxTime { get; set; }
    public TimeSpan AvgTime { get; set; }
}