using KL.Manager.API.Application.Enums;

namespace KL.Manager.API.Application.Models.Responses.Dashboard;

public record CallAnalysisResponse(
    PeriodTypes PeriodType,
    IReadOnlyCollection<CallAnalysisItem> Items);