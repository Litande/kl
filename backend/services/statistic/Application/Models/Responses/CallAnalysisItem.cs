namespace Plat4Me.Dial.Statistic.Api.Application.Models.Responses;

public record CallAnalysisItem(
    string Country,
    string Code,
    int Amount,
    string Rate,
    string AvgTime);