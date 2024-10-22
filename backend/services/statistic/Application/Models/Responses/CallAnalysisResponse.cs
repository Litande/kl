using KL.Statistics.Application.Common.Enums;

namespace KL.Statistics.Application.Models.Responses;

public record CallAnalysisResponse(
    PeriodTypes PeriodType,
    IReadOnlyCollection<CallAnalysisItem> Items);