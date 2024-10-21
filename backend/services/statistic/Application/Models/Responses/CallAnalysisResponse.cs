using Plat4Me.Dial.Statistic.Api.Application.Common.Enums;

namespace Plat4Me.Dial.Statistic.Api.Application.Models.Responses;

public record CallAnalysisResponse(
    PeriodTypes PeriodType,
    IReadOnlyCollection<CallAnalysisItem> Items);