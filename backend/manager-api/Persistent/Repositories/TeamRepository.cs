using Microsoft.EntityFrameworkCore;
using Plat4Me.DialClientApi.Application.Extensions;
using Plat4Me.DialClientApi.Application.Models.Responses.Team;
using Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialClientApi.Persistent.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly DialDbContext _context;
    private readonly IAgentCacheRepository _agentCacheRepository;

    public TeamRepository(DialDbContext context, IAgentCacheRepository agentCacheRepository)
    {
        _context = context;
        _agentCacheRepository = agentCacheRepository;
    }

    public async Task<TeamsResponse> GetAll(
        long currentClientId,
        CancellationToken ct = default)
    {
        var entities = await _context.Teams
            .Where(r => r.ClientId == currentClientId)
            .ToArrayAsync(ct);

        var items = entities
            .Select(r => r.ToResponse());

        return new TeamsResponse(items);
    }

    public async Task<TeamsExtraInfoResponse> GetAllWithExtraInfo(
        long currentClientId,
        CancellationToken ct = default)
    {
        var entities = await _context.Teams
            .Where(r => r.ClientId == currentClientId)
            .AsNoTracking()
            .Include(x => x.Agents)
            .ToArrayAsync(ct);

        var agentIds = entities
            .SelectMany(x => x.Agents)
            .Select(x => x.UserId)
            .Distinct();

        var cachedAgents = await _agentCacheRepository.GetAgents(agentIds);

        var items = entities
            .Select(r => r.ToResponse(cachedAgents));

        return new TeamsExtraInfoResponse(items);
    }
}
