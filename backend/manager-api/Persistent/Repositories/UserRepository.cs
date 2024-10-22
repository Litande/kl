using KL.Manager.API.Application.Enums;
using KL.Manager.API.Application.Extensions;
using KL.Manager.API.Application.Models.Requests;
using KL.Manager.API.Application.Models.Requests.Agents;
using KL.Manager.API.Application.Models.Responses;
using KL.Manager.API.Application.Models.Responses.Agents;
using KL.Manager.API.Persistent.Entities;
using KL.Manager.API.Persistent.Entities.Projections;
using KL.Manager.API.Persistent.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KL.Manager.API.Persistent.Repositories;

public class UserRepository : RepositoryBase, IUserRepository
{
    private readonly KlDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IAgentCacheRepository _agentCacheRepository;

    public UserRepository(
        KlDbContext context,
        UserManager<User> userManager,
        IAgentCacheRepository agentCacheRepository)
    {
        _context = context;
        _userManager = userManager;
        _agentCacheRepository = agentCacheRepository;
    }

    public async Task<AgentsResponse> GetByTeams(
        long clientId,
        AgentsFilterRequest? filters,
        CancellationToken ct = default)
    {
        var agents = await _context.Users
            .ActiveAgents(clientId)
            .Select(r => new TeamAgentProjection
            {
                Id = r.Id,
                FirstName = r.FirstName,
                LastName = r.LastName,
                Name = r.FullName(),
                LeadQueueIds = r.LeadQueues
                    .Select(x => x.Id),
                Score = r.UserTags
                    .Where(p => p.Tag.Status == TagStatusTypes.Enable
                                && (!p.ExpiredOn.HasValue
                                    || p.ExpiredOn >= DateTimeOffset.UtcNow))
                    .Sum(p => p.Tag.Value),
                Tags = r.UserTags
                    .Where(p => p.Tag.Status == TagStatusTypes.Enable
                                && (!p.ExpiredOn.HasValue
                                    || p.ExpiredOn >= DateTimeOffset.UtcNow))
                    .Select(p => new TagProjection
                    {
                        Id = p.Tag.Id,
                        Name = p.Tag.Name,
                        Value = p.Tag.Value,
                        Status = p.Tag.Status,
                    }),
                TeamIds = r.Teams.Select(x => x.Id)
            })
            .ApplyFilters(filters)
            .OrderBy(r => r.Score)
            .ToArrayAsync(ct);

        var agentIds = agents.Select(x => x.Id).ToArray();
        var cachedAgents = await _agentCacheRepository.GetAgents(agentIds);
        var identityUsers = await _userManager.Users
            .Where(r => agentIds
                .Contains(r.Id))
            .ToDictionaryAsync(r => r.Id, r => r, ct);

        foreach (var item in agents)
        {
            item.Email = identityUsers[item.Id].Email;
            item.UserName = identityUsers[item.Id].UserName;

            cachedAgents.TryGetValue(item.Id, out var cachedAgent);

            item.State = cachedAgent?.AgentDisplayStatus ?? AgentStatusTypes.Offline;
            item.ManagerRtcUrl = cachedAgent?.ManagerRtcUrl;
        }

        return new AgentsResponse(agents);
    }

    public async Task<User?> GetById(
        long clientId,
        long userId,
        CancellationToken ct = default)
    {
        var user = await _context.Users
            .Where(r => r.ClientId == clientId
                        && r.Id == userId
                        && !r.DeletedAt.HasValue
                        && r.Status == UserStatusTypes.Active)
            .FirstOrDefaultAsync(ct);

        return user;
    }

    public async Task<AgentProjection?> GetWithTeamsTagsAndQueues(
        long clientId,
        long agentId,
        CancellationToken ct = default)
    {
        var team = await _context.Teams
            .Where(r => r.ClientId == clientId
                        && r.Agents
                            .Any(p => p.Id == agentId))
            .FirstOrDefaultAsync(ct);

        if (team is null)
            throw new Exception($"Team not found for User Id {agentId}");

        var agent = await _context.Users
            .ActiveAgents(clientId, agentId)
            .Select(r => new AgentProjection
            {
                AgentId = r.Id,
                FirstName = r.FirstName,
                LastName = r.LastName,
                TeamIds = r.Teams.Select(p => p.Id),
                LeadQueueIds = r.LeadQueues
                    .Where(p => p.Status == LeadQueueStatusTypes.Enable)
                    .Select(p => p.Id),
                Tags = r.UserTags
                    .Where(p => p.Tag.Status == TagStatusTypes.Enable
                                && (!p.ExpiredOn.HasValue
                                    || p.ExpiredOn >= DateTimeOffset.UtcNow))
                    .Select(p => new TagProjection
                    {
                        Id = p.TagId,
                        Name = p.Tag.Name,
                        Value = p.Tag.Value,
                        Status = p.Tag.Status,
                    }),
                Score = r.UserTags
                    .Where(p => p.Tag.Status == TagStatusTypes.Enable
                                && (!p.ExpiredOn.HasValue
                                    || p.ExpiredOn >= DateTimeOffset.UtcNow))
                    .Sum(p => p.Tag.Value),
            })
            .FirstOrDefaultAsync(ct);

        if (agent is null) return null;

        var identityUser = await _userManager.FindByIdAsync(agent.AgentId.ToString());
        if (identityUser is null) return null;

        agent.Email = identityUser.Email;
        agent.UserName = identityUser.UserName;

        var cachedAgent = await _agentCacheRepository.GetAgent(agent.AgentId);
        agent.Status = cachedAgent?.AgentDisplayStatus
                       ?? AgentStatusTypes.Offline;

        return agent;
    }

    public async Task<AgentProjection?> Create(
        long clientId,
        long userId,
        CreateAgentRequest request,
        CancellationToken ct = default)
    {
        var agent = new User
        {
            Email = request.Email,
            UserName = request.Email,
            EmailConfirmed = true,
            ClientId = clientId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            CreatedAt = DateTimeOffset.UtcNow,
            RoleType = RoleTypes.Agent,
        };

        await SyncAgentTeams(agent, request.TeamIds, ct);
        await SyncAgentLeadQueues(agent, request.LeadQueueIds, ct);
        await SyncAgentTags(agent, request.TagIds, ct);

        var identityResult = await _userManager.CreateAsync(agent, request.Password);
        identityResult.ValidateIdentityResult();

        var identityUserRoleResult = await _userManager.AddToRoleAsync(agent, "AGENT");
        identityUserRoleResult.ValidateIdentityResult();

        return await GetWithTeamsTagsAndQueues(clientId, agent.Id, ct);
    }

    public async Task<AgentProjection?> Update(
        long clientId,
        long agentId,
        UpdateAgentRequest request,
        CancellationToken ct = default)
    {
        var agent = await _context.Users
            .ActiveAgents(clientId, agentId)
            .Include(r => r.Teams)
            .Include(r => r.LeadQueues)
            .Include(r => r.UserTags)
            .FirstOrDefaultAsync(ct);

        if (agent is null) return null;

        await SyncAgentTeams(agent, request.TeamIds, ct);
        await SyncAgentLeadQueues(agent, request.LeadQueueIds, ct);
        await SyncAgentTags(agent, request.TagIds, ct);

        var identityUser = await _userManager.FindByIdAsync(agent.Id.ToString());
        if (identityUser is null) return null;

        var setUserNameIdentityResult = await _userManager.SetUserNameAsync(identityUser, request.UserName);
        setUserNameIdentityResult.ValidateIdentityResult();

        var setEmailIdentityResult = await _userManager.SetEmailAsync(identityUser, request.Email);
        setEmailIdentityResult.ValidateIdentityResult();

        agent.FirstName = request.FirstName;
        agent.LastName = request.LastName;
        await _context.SaveChangesAsync(ct);

        return await GetWithTeamsTagsAndQueues(clientId, agentId, ct);
    }

    public async Task<PaginatedResponse<AgentTagsProjection>> GetByTagIds(
        long clientId,
        PaginationRequest pagination,
        IReadOnlyCollection<long>? tagIds = null,
        CancellationToken ct = default)
    {
        var q = _context.Users.ActiveAgents(clientId);

        if (tagIds is not null
            && tagIds.Any())
        {
            q = q.Where(r => r.UserTags
                .Any(p => tagIds
                    .Contains(p.TagId)));
        }

        var rq = q.Select(r => new AgentTagsProjection
        {
            Id = r.Id,
            FirstName = r.FirstName,
            LastName = r.LastName,
            TeamName = r.Teams.First().Name,
            Score = r.UserTags
                .Where(p => p.Tag.Status == TagStatusTypes.Enable
                            && (!p.ExpiredOn.HasValue
                                || p.ExpiredOn >= DateTimeOffset.UtcNow))
                .Sum(p => p.Tag.Value),
            Tags = r.UserTags
                .Where(p => p.Tag.Status == TagStatusTypes.Enable
                            && (!p.ExpiredOn.HasValue
                                || p.ExpiredOn >= DateTimeOffset.UtcNow))
                .Select(p => new TagProjection
                {
                    Id = p.TagId,
                    Name = p.Tag.Name,
                    Value = p.Tag.Value,
                    Status = p.Tag.Status,
                })
        });

        return await CreatePaginatedResponse(rq, pagination, ct);
    }

    public async Task<IEnumerable<TagProjection>> UpdateTags(
        long clientId,
        long agentId,
        IReadOnlyCollection<long>? tagIds,
        CancellationToken ct = default)
    {
        var agent = await _context.Users
            .ActiveAgents(clientId, agentId)
            .Include(r => r.UserTags)
            .ThenInclude(r => r.Tag)
            .FirstOrDefaultAsync(ct);

        if (agent is null)
            throw new Exception($"Agent Id {agentId} not found");

        await SyncAgentTags(agent, tagIds, ct);
        await _context.SaveChangesAsync(ct);

        var items = agent.UserTags
            .Where(r => r.Tag.Status == TagStatusTypes.Enable
                        && (!r.ExpiredOn.HasValue
                            || r.ExpiredOn >= DateTimeOffset.UtcNow))
            .Select(r => new TagProjection
            {
                Id = r.Tag.Id,
                Name = r.Tag.Name,
                Value = r.Tag.Value,
                Status = r.Tag.Status,
            });

        return items;
    }

    public async Task<IReadOnlyCollection<AgentInfoProjection>> GetAgentInfoByIds(
        long clientId,
        IEnumerable<long> agentIds,
        CancellationToken ct = default)
    {
        var items = await _context.Users
            .Include(x => x.Teams)
            .Where(r => r.ClientId == clientId
                        && r.RoleType == RoleTypes.Agent
                        && agentIds.Contains(r.Id))
            .SelectAgentInfoProjection()
            .ToArrayAsync(ct);

        return items;
    }

    public async Task<AgentInfoProjection?> GetAgentInfoById(long clientId, long agentId, CancellationToken ct = default)
    {
        return await _context.Users
           .Include(x => x.Teams)
           .Where(r => r.ClientId == clientId
                       && r.RoleType == RoleTypes.Agent
                       && r.Id == agentId)
           .SelectAgentInfoProjection()
           .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyCollection<AgentInfoProjection>> GetAgentInfoByClientId(
        long clientId,
        CancellationToken ct = default)
    {
        var items = await _context.Users
            .Include(x => x.Teams)
            .Where(r => r.ClientId == clientId
                        && r.RoleType == RoleTypes.Agent)
            .SelectAgentInfoProjection()
            .ToArrayAsync(ct);

        return items;
    }

    private async Task SyncAgentTeams(
        User agent,
        IReadOnlyCollection<long> teamIds,
        CancellationToken ct = default)
    {
        if (!teamIds.Any())
            throw new Exception("At least one Team Id should be selected");

        var teams = await _context.Teams
            .Where(r => r.ClientId == agent.ClientId
                        && teamIds.Contains(r.Id))
            .ToArrayAsync(ct);

        var notFoundTeamIds = teamIds
            .Where(r => !teams
                .Any(p => p.Id == r))
            .ToArray();

        if (notFoundTeamIds.Any())
            throw new Exception($"Team Ids {string.Join(", ", notFoundTeamIds)} not found");

        agent.Teams.Clear();

        foreach (var item in teams) agent.Teams.Add(item);
    }

    private async Task SyncAgentLeadQueues(
        User agent,
        IReadOnlyCollection<long>? leadQueueIds,
        CancellationToken ct = default)
    {
        if (leadQueueIds is null || !leadQueueIds.Any())
        {
            agent.LeadQueues.Clear();
            return;
        }

        var leadQueues = await _context.LeadQueues
            .Where(r => r.ClientId == agent.ClientId
                        && leadQueueIds.Contains(r.Id))
            .ToArrayAsync(ct);

        var notFoundLeadQueueIds = leadQueueIds
            .Where(r => !leadQueues
                .Any(p => p.Id == r))
            .ToArray();

        if (notFoundLeadQueueIds.Any())
            throw new Exception($"LeadQueue Ids {string.Join(", ", notFoundLeadQueueIds)} not found");

        agent.LeadQueues.Clear();

        foreach (var item in leadQueues) agent.LeadQueues.Add(item);
    }

    private async Task SyncAgentTags(
        User agent,
        IReadOnlyCollection<long>? tagIds,
        CancellationToken ct = default)
    {
        if (tagIds is null || !tagIds.Any())
        {
            agent.UserTags.Clear();
            return;
        }

        var userTagsToRemove = agent.UserTags
            //.Where(r => r.Tag.Status == TagStatusTypes.Disable
            //            && (r.ExpiredOn.HasValue
            //                || r.ExpiredOn < DateTimeOffset.UtcNow))
            // TODO check if requires filter by enable
            .Where(r => !tagIds
                .Contains(r.TagId));

        foreach (var item in userTagsToRemove) agent.UserTags.Remove(item);

        var existTagIds = agent.UserTags.Select(r => r.TagId);
        var tagIdsToAdd = tagIds.Except(existTagIds).ToArray();
        var tags = await GetTagsByIds(agent.ClientId, tagIdsToAdd, ct: ct);
        var notFoundTagIds = tagIdsToAdd
            .Where(r => !tags
                .Any(p => p.Id == r))
            .ToArray();

        if (notFoundTagIds.Any())
            throw new Exception($"Tag Ids {string.Join(", ", notFoundTagIds)} not found");

        var newUserTags = UserRepositoryExtensions.PrepareUserTags(tags);

        foreach (var item in newUserTags) agent.UserTags.Add(item);
    }

    private async Task<IEnumerable<Tag>> GetTagsByIds(
        long clientId,
        IEnumerable<long> tagIds,
        TagStatusTypes? status = TagStatusTypes.Enable,
        CancellationToken ct = default)
    {
        var entities = await _context.Tags
            .Where(r => r.ClientId == clientId
                        && r.Status == status
                        && tagIds.Contains(r.Id))
            .ToArrayAsync(ct);

        return entities;
    }
}

internal static class UserRepositoryExtensions
{
    public static IQueryable<User> ActiveAgents(
        this IQueryable<User> context,
        long clientId,
        long? userId = null)
    {
        var q = context
            .Where(r => r.ClientId == clientId
                        && !r.DeletedAt.HasValue
                        && r.RoleType == RoleTypes.Agent
                        && r.Status == UserStatusTypes.Active);

        if (userId.HasValue)
            q = q.Where(r => r.Id == userId);

        return q;
    }

    public static IQueryable<TeamAgentProjection> ApplyFilters(
        this IQueryable<TeamAgentProjection> q,
        AgentsFilterRequest? filters = null)
    {
        if (filters is null) return q;

        if (filters.AgentIds is not null
            && filters.AgentIds.Any())
        {
            q = q.Where(r => filters.AgentIds
                .Contains(r.Id));
        }

        if (filters.TagIds is not null
            && filters.TagIds.Any())
        {
            q = q.Where(r => r.Tags
                .Any(p => filters.TagIds
                    .Contains(p.Id)));
        }

        if (filters.AgentStatuses is not null
            && filters.AgentStatuses.Any())
        {
            q = q.Where(r => filters.AgentStatuses
                .Contains(r.State));
        }

        if (filters.GroupIds is not null
            && filters.GroupIds.Any())
        {
            q = q.Where(r => r.LeadQueueIds
                .Any(p => filters.GroupIds
                    .Contains(p)));
        }

        return q;
    }

    public static ICollection<UserTag> PrepareUserTags(IEnumerable<Tag> tags)
    {
        var items = tags
            .Select(r => new UserTag
            {
                Tag = r,
                ExpiredOn = CalculateTagExpiredOn(r.LifetimeSeconds),
            })
            .ToArray();

        return items;
    }

    public static DateTimeOffset? CalculateTagExpiredOn(int? lifetimeSeconds) =>
        lifetimeSeconds.HasValue
            ? DateTimeOffset.UtcNow.AddSeconds(lifetimeSeconds.Value)
            : null;

    public static void ValidateIdentityResult(this IdentityResult identityResult)
    {
        if (!identityResult.Succeeded)
            throw new Exception(string
                .Join(Environment.NewLine, identityResult.Errors
                    .Select(r => r.Description)));
    }

    public static IQueryable<AgentInfoProjection> SelectAgentInfoProjection(this IQueryable<User> q)
    {
        return q.Select(r => new AgentInfoProjection
        {
            AgentId = r.Id,
            FirstName = r.FirstName,
            LastName = r.LastName,
            //BrandName = r.Client.Name, // TODO get from client?
            LeadQueues = r.LeadQueues
                .Select(p => p.Name),
            Score = r.UserTags
                .Where(p => p.Tag.Status == TagStatusTypes.Enable
                            && (!p.ExpiredOn.HasValue
                                || p.ExpiredOn >= DateTimeOffset.UtcNow))
                .Sum(p => p.Tag.Value),
            TeamIds = r.Teams.Select(x => x.Id),
            Tags = r.UserTags.Where(p => p.Tag.Status == TagStatusTypes.Enable
                                         && (!p.ExpiredOn.HasValue
                                             || p.ExpiredOn >= DateTimeOffset.UtcNow))
                .Select(p => new TagProjection
                {
                    Id = p.Tag.Id,
                    Name = p.Tag.Name,
                    Value = p.Tag.Value,
                    Status = p.Tag.Status,
                })
        });
    }
}
