using KL.Manager.API.Application.Enums;

namespace KL.Manager.API.Application.Models.Responses.LeadGroups;

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