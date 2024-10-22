using System.Text.Json;
using System.Text.Json.Serialization;
using KL.Provider.Leads.Application.Enums;
using KL.Provider.Leads.Application.Models;
using KL.Provider.Leads.Persistent.Entities;
using KL.Provider.Leads.Persistent.Repositories.Interfaces;

namespace KL.Provider.Leads.Persistent.Repositories;

public class LeadHistoryRepository : ILeadHistoryRepository
{
    private readonly DialDbContext _context;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };

    public LeadHistoryRepository(DialDbContext context)
    {
        _context = context;
    }

    public async Task AddLeadHistory(
        long leadId,
        IEnumerable<ValueChanges<object?>> changes,
        DateTimeOffset createdAt,
        long? createdBy = null,
        CancellationToken ct = default)
    {
        var historyEntry = new LeadHistory
        {
            LeadId = leadId,
            ActionType = LeadHistoryActionType.Data,
            Changes = JsonSerializer.Serialize(
                new LeadHistoryChangesDto<object?>
                {
                    Properties = changes.ToList()
                },
                _jsonOptions),
            CreatedAt = createdAt,
            CreatedBy = createdBy
        };

        await _context.LeadHistory.AddAsync(historyEntry, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task AddLeadHistory<T>(
        long leadId,
        LeadHistoryActionType type,
        ValueChanges<T?> changes,
        DateTimeOffset createdAt,
        long? createdBy = null,
        CancellationToken ct = default)
    {
        var historyEntry = new LeadHistory
        {
            LeadId = leadId,
            ActionType = type,
            Changes = JsonSerializer.Serialize(
                new LeadHistoryChangesDto<T?>
                {
                    Properties = new List<ValueChanges<T?>>() { changes }
                },
                _jsonOptions),
            CreatedAt = createdAt,
            CreatedBy = createdBy
        };

        await _context.LeadHistory.AddAsync(historyEntry, ct);
        await _context.SaveChangesAsync(ct);
    }
}