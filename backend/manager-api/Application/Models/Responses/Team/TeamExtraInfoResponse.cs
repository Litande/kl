namespace Plat4Me.DialClientApi.Application.Models.Responses.Team;

public record TeamExtraInfoResponse(long TeamId,
    string Name,
    int TotalAgentCount,
    int OnlineAgentCount,
    int OfflineAgentCount);