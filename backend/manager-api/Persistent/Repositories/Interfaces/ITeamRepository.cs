using Plat4Me.DialClientApi.Application.Models.Responses.Team;

namespace Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

public interface ITeamRepository
{
    Task<TeamsResponse> GetAll(long currentClientId, CancellationToken ct = default);
    Task<TeamsExtraInfoResponse> GetAllWithExtraInfo(long currentClientId, CancellationToken ct = default);
}
