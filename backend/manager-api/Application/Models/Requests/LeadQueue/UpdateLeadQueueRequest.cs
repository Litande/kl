namespace Plat4Me.DialClientApi.Application.Models.Requests.LeadQueue;

public class UpdateLeadQueueRequest
{
    public double? DropRateUpperThreshold { get; set; }
    public double? DropRateLowerThreshold { get; set; }
    public int DropRatePeriod { get; set; }
    public double RatioStep { get; set; }
    public double? MaxRatio { get; set; }
    public double? MinRatio { get; set; }
    public int RatioFreezeTime { get; set; }
}