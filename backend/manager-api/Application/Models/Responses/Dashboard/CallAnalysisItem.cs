namespace Plat4Me.DialClientApi.Application.Models.Responses.Dashboard;

public record CallAnalysisItem(
    string Country,
    string Code,
    int Amount,
    string Rate,
    string AvgTime);