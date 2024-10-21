namespace Plat4Me.DialLeadCaller.Application.Models.LeadStatisticCache;

public class StatisticItemCache
{
    public long Amount { get; set; }
    public string? Country { get; set; }
    public List<LeadItemCache> Leads { get; set; }
    public TimeSpan MaxTime { get; set; }
    public TimeSpan AvgTime { get; set; }
}