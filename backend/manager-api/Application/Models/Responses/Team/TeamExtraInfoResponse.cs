namespace KL.Manager.API.Application.Models.Responses.Team;

public record TeamExtraInfoResponse(long TeamId,
    string Name,
    int TotalAgentCount,
    int OnlineAgentCount,
    int OfflineAgentCount);