using Plat4Me.DialClientApi.Application.Enums;

namespace Plat4Me.DialClientApi.Application.Models.Responses.Dashboard;

public record CallAnalysisResponse(
    PeriodTypes PeriodType,
    IReadOnlyCollection<CallAnalysisItem> Items);