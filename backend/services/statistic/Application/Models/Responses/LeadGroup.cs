using Plat4Me.Dial.Statistic.Api.Application.Common.Enums;

namespace Plat4Me.Dial.Statistic.Api.Application.Models.Responses;

public record LeadGroup(
    string Name,
    int Agents,
    double Ratio,
    string DropRatePercentage,
    int LeadsCount,
    long LeadQueueId,
    LeadQueueTypes Type,
    double? DropRateUpperThreshold,
    double? DropRateLowerThreshold,
    int DropRatePeriod,
    double RatioStep,
    int? RatioFreezeTime,
    double? MaxRatio = null,
    double? MinRatio = null);