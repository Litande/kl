using KL.Manager.API.Application.Models.Responses.Team;

namespace KL.Manager.API.Persistent.Repositories.Interfaces;

public interface ITeamRepository
{
    Task<TeamsResponse> GetAll(long currentClientId, CancellationToken ct = default);
    Task<TeamsExtraInfoResponse> GetAllWithExtraInfo(long currentClientId, CancellationToken ct = default);
}
