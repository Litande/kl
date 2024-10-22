using System.Text.Json;
using KL.Manager.API.Application.Common;
using KL.Manager.API.Application.Enums;
using KL.Manager.API.Application.Extensions;
using KL.Manager.API.Application.Models;
using KL.Manager.API.Application.Models.Requests;
using KL.Manager.API.Application.Models.Requests.Leads;
using KL.Manager.API.Application.Models.Responses;
using KL.Manager.API.Application.Models.Responses.Leads;
using KL.Manager.API.Application.Models.Responses.UserFilter;
using KL.Manager.API.Persistent.Entities;
using KL.Manager.API.Persistent.Entities.Projections;
using KL.Manager.API.Persistent.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KL.Manager.API.Persistent.Repositories;

public class LeadRepository : RepositoryBase, ILeadRepository
{
    private readonly KlDbContext _context;
    private readonly ILeadLastCacheRepository _leadLastCacheRepository;
    private readonly ILogger<LeadRepository> _logger;
    private readonly IDataSourceRepository _dataSourceRepository;

    public LeadRepository(
        KlDbContext context,
        ILeadLastCacheRepository leadLastCacheRepository,
        ILogger<LeadRepository> logger,
        IDataSourceRepository dataSourceRepository)
    {
        _context = context;
        _leadLastCacheRepository = leadLastCacheRepository;
        _logger = logger;
        _dataSourceRepository = dataSourceRepository;
    }

    public async Task<Lead?> GetById(
        long clientId,
        long leadId,
        CancellationToken ct = default)
    {
        var entity = await _context.Leads
            .Where(r => r.ClientId == clientId
                        && r.Id == leadId)
            .FirstOrDefaultAsync(ct);

        return entity;
    }

    public async Task<Lead?> GetByIdAsNoTracking(
        long clientId,
        long leadId,
        CancellationToken ct = default)
    {
        var entity = await _context.Leads
            .AsNoTracking()
            .Where(r => r.ClientId == clientId
                        && r.Id == leadId)
            .FirstOrDefaultAsync(ct);

        return entity;
    }

    public async Task<LeadShortInfo?> UpdateLeadAssignment(
        long clientId,
        long userId,
        long leadId,
        long? assignedUserId,
        CancellationToken ct = default)
    {
        var entity = await GetById(clientId, leadId, ct);
        if (entity is null)
            return null;

        if (!assignedUserId.HasValue) //TODO check agent validation
        {
            var user = await _context.Users
                .Where(r =>
                    //!r.DeletedAt.HasValue
                    //&& r.RoleType == RoleTypes.Agent &&
                    r.Id == assignedUserId)
                .FirstOrDefaultAsync(ct);

            if (user is null)
                return null;
        }

        var now = DateTime.UtcNow;

        await AddLeadHistories(leadId, LeadHistoryActionType.Data, createdBy: userId, ct,
            new(nameof(entity.AssignedAgentId), entity.AssignedAgentId, null),
            new(nameof(entity.LastUpdateTime), entity.LastUpdateTime, now));

        entity.AssignedAgentId = assignedUserId;
        entity.LastUpdateTime = now;

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Last call agent changed to Id {assignedUserId} for lead id {leadId}",
            assignedUserId, leadId);

        return entity.ToLeadShortInfoResponse();
    }

    public async Task<LeadShortInfo?> UpdateLeadStatus(
        long clientId,
        long userId,
        long leadId,
        LeadStatusTypes status,
        CancellationToken ct = default)
    {
        var entity = await GetById(clientId, leadId, ct);
        if (entity is null)
            return null;

        var now = DateTime.UtcNow;

        await AddLeadHistories(leadId, LeadHistoryActionType.Data, createdBy: userId, ct,
            new ValueChanges<object?>(nameof(entity.LastUpdateTime), entity.LastUpdateTime, now));

        await AddLeadHistories(leadId, LeadHistoryActionType.Status, createdBy: userId, ct,
            new ValueChanges<object?>(nameof(entity.Status), entity.Status, status));

        entity.Status = status;
        entity.LastUpdateTime = now;

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Status changed to {status} for lead id {leadId}", status, leadId);

        return entity.ToLeadShortInfoResponse();
    }

    public async Task<PaginatedResponse<Dictionary<string, JsonElement?>>> SearchLeads(
        long clientId,
        PaginationRequest pagination,
        LeadsFilterRequest? filter = null,
        CancellationToken ct = default)
    {
        var q = _context.Leads
            .Where(r => r.ClientId == clientId);
        if (filter is not null)
        {
            if (!string.IsNullOrEmpty(filter.LeadId))
            {
                var isSuccess = long.TryParse(filter.LeadId, out var leadId);
                if (!isSuccess)
                    throw new ArgumentException($"{filter.LeadId} is wrong id");

                q = q.Where(r => r.Id == leadId);
            }

            if (!string.IsNullOrEmpty(filter.Phone))
            {
                var pattern = FullSearchPattern(filter.Phone);
                q = q.Where(r => EF.Functions.Like(r.Phone, pattern));
            }

            if (!string.IsNullOrEmpty(filter.FirstName))
            {
                var pattern = FullSearchPattern(filter.FirstName);
                q = q.Where(r => EF.Functions.Like(r.FirstName, pattern));
            }

            if (!string.IsNullOrEmpty(filter.LastName))
            {
                var pattern = FullSearchPattern(filter.LastName);
                q = q.Where(r => EF.Functions.Like(r.LastName, pattern));
            }

            if (filter.Country is not null && filter.Country.Any())
            {
                q = q.Where(r => filter.Country.Contains(r.CountryCode!));
            }

            if (filter.LeadStatus is not null && filter.LeadStatus.Any())
            {
                q = q.Where(r => filter.LeadStatus.Contains(r.Status));
            }

            if (filter.AssignedAgent is not null && filter.AssignedAgent.Any())
            {
                q = q.Where(r => filter.AssignedAgent.Contains(r.AssignedAgentId!.Value));
            }

            if (!string.IsNullOrEmpty(filter.Email))
            {
                var pattern = FullSearchPattern(filter.Email);
                q = q.Where(r => EF.Functions.Like(r.Email!, pattern));
            }

            if (!string.IsNullOrEmpty(filter.Brand))
            {
                var pattern = FullSearchPattern(filter.Brand);
                q = q.Where(r => EF.Functions.Like(r.DataSource.Name, pattern));
            }
        }

        if (filter?.TotalCalls is not null)
        {
            q = filter.TotalCalls.Operation switch
            {
                FilterComparisonOperation.Equal => q.Where(r => r.CallDetailRecords.Count(x =>
                    x.CallHangupStatus == CallFinishReasons.CallFinishedByLead ||
                    x.CallHangupStatus == CallFinishReasons.CallFinishedByAgent) == filter.TotalCalls.Value),
                FilterComparisonOperation.MoreThan => q.Where(r => r.CallDetailRecords.Count(x =>
                    x.CallHangupStatus == CallFinishReasons.CallFinishedByLead ||
                    x.CallHangupStatus == CallFinishReasons.CallFinishedByAgent) > filter.TotalCalls.Value),
                FilterComparisonOperation.LessThan => q.Where(r => r.CallDetailRecords.Count(x =>
                    x.CallHangupStatus == CallFinishReasons.CallFinishedByLead ||
                    x.CallHangupStatus == CallFinishReasons.CallFinishedByAgent) < filter.TotalCalls.Value),
                _ => q
            };
        }

        var entities = await q.Select(r => new LeadSearchProjection
        {
            LeadId = r.Id,
            PhoneNumber = r.Phone,
            Country = r.CountryCode,
            FirstName = r.FirstName,
            LastName = r.LastName,
            BrandName = r.DataSource.Name,
            RegistrationTime = r.RegistrationTime,
            Email = r.Email,
            Metadata = r.Metadata,
            DataSourceId = r.DataSourceId,
            TotalCalls = r.CallDetailRecords.Count(x => x.CallHangupStatus == CallFinishReasons.CallFinishedByLead
                                                        || x.CallHangupStatus == CallFinishReasons.CallFinishedByAgent),
            LeadStatus = r.Status.ToDescription(),
            IsBlocked = r.LeadBlacklist.Any(),
            AssignedAgent = r.LastCallAgent != null
                ? new AssignedAgentProjection
                {
                    AgentId = r.LastCallAgent.Id,
                    FirstName = r.LastCallAgent.FirstName,
                    LastName = r.LastCallAgent.LastName
                }
                : null
        }).ToDictionaryAsync(x => x.LeadId, x => x, ct);

        await FillLeadsWeights(entities);

        var leads = entities.Values.AsEnumerable();

        if (filter?.WeightMoreThan is not null)
        {
            var isSuccess = long.TryParse(filter.WeightMoreThan, out var score);
            if (!isSuccess)
                throw new ArgumentException($"{filter.WeightMoreThan} is wrong score");

            leads = leads.Where(x => x.LeadScore >= score);
        }

        if (filter?.WeightLessThan is not null)
        {
            var isSuccess = long.TryParse(filter.WeightLessThan, out var score);
            if (!isSuccess)
                throw new ArgumentException($"{filter.WeightLessThan} is wrong score");

            leads = leads.Where(x => x.LeadScore <= score);
        }

        var paginated = CreatePaginatedResponse(leads.ToArray(), pagination);
        var nodes = await AddExtraFields(paginated.Items, filter, ct);

        return new PaginatedResponse<Dictionary<string, JsonElement?>>
        {
            Items = nodes,
            PageSize = paginated.PageSize,
            Page = paginated.Page,
            TotalCount = paginated.TotalCount,
        };
    }

    private async Task<IEnumerable<Dictionary<string, JsonElement?>>> AddExtraFields(
        IEnumerable<LeadSearchProjection> paginated,
        LeadsFilterRequest? filter,
        CancellationToken ct = default)
    {
        var dataSources = (await _dataSourceRepository.GetAllDataSource(ct))
            .ToDictionary(x => x.Id, x => x.Metadata is null
                ? null
                : JsonSerializer.Deserialize<FilterResponse>(x.Metadata, JsonSettingsExtensions.Default)!
                    .Fields
                    .Select(y => y.FieldName).ToHashSet());

        var filterGroups = filter?.Groups?
            .ToDictionary(x => x.Name, x => x.Fields.Select(y => y.Name).ToHashSet());

        return paginated.Select(x =>
        {
            var entityNodes =
                JsonSerializer.Deserialize<Dictionary<string, JsonElement?>>(
                    JsonSerializer.Serialize(x, JsonSettingsExtensions.Default))!;

            var dataSourceMetaFields = dataSources[x.DataSourceId];
            if (dataSourceMetaFields is null)
            {
                _logger.LogWarning("Metadata is null in the datasource {DataSourceId}", x.DataSourceId);
                entityNodes.Remove(ToCamelCase(nameof(LeadSearchProjection.Metadata)));
                entityNodes.Remove(ToCamelCase(nameof(LeadSearchProjection.DataSourceId)));
                return entityNodes;
            }

            Dictionary<string, JsonElement?>? metaNodes = null;
            if (x.Metadata is not null)
                metaNodes = JsonSerializer.Deserialize<Dictionary<string, JsonElement?>>(x.Metadata!);

            if (filterGroups?.ContainsKey("Custom Field") ?? false)
            {
                var filterGroup = filterGroups["Custom Field"];
                foreach (var fieldName in filterGroup)
                {
                    if (!dataSourceMetaFields?.Contains(fieldName) ?? false)
                        throw new ArgumentException($"{fieldName} is wrong field");

                    JsonElement? element = null;
                    metaNodes?.TryGetValue(fieldName, out element);
                    entityNodes.Add(fieldName, element);
                }
            }
            else
            {
                foreach (var fieldName in dataSourceMetaFields)
                {
                    JsonElement? element = null;
                    metaNodes?.TryGetValue(fieldName, out element);
                    entityNodes.Add(fieldName, element);
                }
            }

            entityNodes.Remove(ToCamelCase(nameof(LeadSearchProjection.Metadata)));
            entityNodes.Remove(ToCamelCase(nameof(LeadSearchProjection.DataSourceId)));
            return entityNodes;
        });
    }

    private static string ToCamelCase(string str)
    {
        return char.ToLowerInvariant(str[0]) + str.Substring(1);
    }

    private async Task FillLeadsWeights(IDictionary<long, LeadSearchProjection> collection)
    {
        var cachedLeads = await _leadLastCacheRepository.GetLeads(collection.Keys);
        foreach (var cachedLead in cachedLeads)
        {
            collection[cachedLead.Key].LeadScore = cachedLead.Value.Score.GetValueOrDefault();
        }
    }

    public async Task<IDictionary<long, LeadInfoProjection>> GetLeadInfoByIds(
        long clientId,
        IEnumerable<long> leadIds,
        CancellationToken ct = default)
    {
        var items = await _context.Leads
            .Include(x => x.DataSource)
            .Where(r => r.ClientId == clientId
                        && leadIds.Contains(r.Id))
            .Select(r => new LeadInfoProjection
            {
                LeadId = r.Id,
                FirstName = r.FirstName,
                LastName = r.LastName,
                Phone = r.Phone,
                RegistrationTime = r.RegistrationTime,
                LeadStatus = r.Status,
                CountryCode = r.CountryCode,
                BrandName = r.DataSource.Name,
                // AffiliateId = r.,
            })
            .ToDictionaryAsync(r => r.LeadId, r => r, ct);

        return items;
    }

    public async Task<LeadInfoProjection?> GetLeadInfoById(long clientId, long leadId, CancellationToken ct = default)
    {
        return await _context.Leads
            .Include(x => x.DataSource)
            .Where(r => r.ClientId == clientId
                        && r.Id == leadId)
            .Select(r => new LeadInfoProjection
            {
                LeadId = r.Id,
                FirstName = r.FirstName,
                LastName = r.LastName,
                Phone = r.Phone,
                RegistrationTime = r.RegistrationTime,
                LeadStatus = r.Status,
                CountryCode = r.CountryCode,
                BrandName = r.DataSource.Name,
                // AffiliateId = r.,
            }).FirstOrDefaultAsync(ct);
    }

    public async Task AddLeadHistories(
        long leadId,
        LeadHistoryActionType actionType,
        long? createdBy = null,
        CancellationToken ct = default,
        params ValueChanges<object?>[] changes)
    {
        var changesDto = new LeadHistoryChangesDto<object?>
        {
            Properties = changes.ToList()
        };

        var historyEntry = new LeadHistory
        {
            LeadId = leadId,
            ActionType = actionType,
            Changes = JsonSerializer.Serialize(changesDto, JsonSettingsExtensions.Default),
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = createdBy,
        };

        await _context.LeadHistory.AddAsync(historyEntry, ct);
    }

    [Obsolete("Should be removed")]
    public async Task<LeadShortInfo?> ResetLead(
        long clientId,
        long leadId,
        CancellationToken ct = default)
    {
        var entity = await GetById(clientId, leadId, ct);
        if (entity is null) return null;

        entity.Status = LeadStatusTypes.NewLead;
        entity.SystemStatus = null;
        await _context.SaveChangesAsync(ct);

        return entity.ToLeadShortInfoResponse();
    }
}