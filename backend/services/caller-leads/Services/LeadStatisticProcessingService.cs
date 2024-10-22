using System.Globalization;
using System.Text.Json;
using KL.Caller.Leads.App;
using KL.Caller.Leads.Enums;
using KL.Caller.Leads.Extensions;
using KL.Caller.Leads.Models;
using KL.Caller.Leads.Models.Entities;
using KL.Caller.Leads.Models.LeadStatisticCache;
using KL.Caller.Leads.Models.Messages;
using KL.Caller.Leads.Repositories;
using KL.Caller.Leads.Services.Contracts;
using Microsoft.Extensions.Options;

namespace KL.Caller.Leads.Services;

public class LeadStatisticProcessingService : ILeadStatisticProcessing
{
    private readonly ILogger<LeadStatisticProcessingService> _logger;
    private readonly ICDRRepository _cdrRepository;
    private readonly ILeadCacheRepository _leadCacheRepository;
    private readonly ILeadStatisticCacheRepository _statisticCacheRepository;
    private readonly PubSubjects _pubSubjects;
    private readonly INatsPublisher _natsPublisher;
    private readonly ISettingsRepository _settingsRepository;

    private static readonly Dictionary<string, string> Countries = CultureInfo
        .GetCultures(CultureTypes.SpecificCultures)
        .Select(culture => new RegionInfo(culture.Name))
        .DistinctBy(x => x.TwoLetterISORegionName)
        .ToDictionary(x => x.TwoLetterISORegionName, x => x.EnglishName);

    public LeadStatisticProcessingService(
        ILogger<LeadStatisticProcessingService> logger,
        ICDRRepository cdrRepository,
        ILeadCacheRepository leadLastCacheRepository,
        ILeadStatisticCacheRepository statisticCacheRepository,
        IOptions<PubSubjects> pubSubjects,
        INatsPublisher natsPublisher,
        ISettingsRepository settingsRepository)
    {
        _logger = logger;
        _cdrRepository = cdrRepository;
        _leadCacheRepository = leadLastCacheRepository;
        _statisticCacheRepository = statisticCacheRepository;
        _natsPublisher = natsPublisher;
        _settingsRepository = settingsRepository;
        _pubSubjects = pubSubjects.Value;
    }

    public async Task ProcessAll(CancellationToken ct = default)
    {
        var settings = await _settingsRepository
            .GetValuesForAllClients(SettingTypes.LeadStatistic, ct);

        foreach (var keyValue in settings)
        {
            var (clientId, jsonSettings) = keyValue;
            var leadStatuses = DeserializeToSettings(jsonSettings, clientId);

            if (leadStatuses is null)
                continue;

            await ProcessCalculations(clientId, leadStatuses, ct);
        }
    }

    public async Task Process(long clientId, CancellationToken ct = default)
    {
        var jsonSettings = await _settingsRepository.GetValue(SettingTypes.LeadStatistic, clientId, ct);
        var leadStatuses = DeserializeToSettings(jsonSettings, clientId);

        if (leadStatuses is null)
            return;

        await ProcessCalculations(clientId, leadStatuses, ct);
    }

    private async Task ProcessCalculations(long clientId, IEnumerable<LeadStatusTypes> leadStatuses,
        CancellationToken ct = default)
    {
        var callDetailRecords = await _cdrRepository
            .GetCallDetailsForCalculations(
                clientId,
                DateTimeOffset.UtcNow.Date,
                leadStatuses,
                ct);

        var leadStatistics = await GetCalculationLeadStatistics(callDetailRecords);

        await _statisticCacheRepository.UpdateStatistics(clientId, leadStatistics);

        var message = new LeadsStatisticsUpdateMessage(clientId);
        await _natsPublisher.PublishAsync(_pubSubjects.LeadsStatisticsUpdate, message);
    }

    private async Task<List<StatisticItemCache>> GetCalculationLeadStatistics(
        IReadOnlyCollection<CallDetailRecord> callDetailRecords)
    {
        var leadIds = callDetailRecords.Select(x => x.LeadId!.Value).ToArray();

        _logger.LogInformation("Start Lead Statistic processing with lead Ids: {leadIds}",
            string.Join(", ", leadIds));

        var cachedLeads = (await _leadCacheRepository.GetLeads(leadIds))
            .ToDictionary(x => x.LeadId, x => x);

        return callDetailRecords
            .GroupBy(x => x.LeadCountry!)
            .Select(x =>
            {
                var leadsByCountry = ConvertToLeadItemCaches(x, cachedLeads);

                Countries.TryGetValue(x.Key, out var countryEnglishName);
                var maxTime = x.Max(y => y.OriginatedAt - y.Lead?.FirstTimeQueuedOn)?.TotalSeconds;
                var avgTime = x.Average(y => (y.OriginatedAt - y.Lead?.FirstTimeQueuedOn)?.TotalSeconds);

                return new StatisticItemCache
                {
                    Country = countryEnglishName ?? x.Key,
                    Amount = leadsByCountry.Count,
                    Leads = leadsByCountry,
                    MaxTime = TimeSpan.FromSeconds(maxTime ?? 0),
                    AvgTime = TimeSpan.FromSeconds(avgTime ?? 0)
                };
            })
            .ToList();
    }

    private static List<LeadItemCache> ConvertToLeadItemCaches(
        IEnumerable<CallDetailRecord> groups,
        IReadOnlyDictionary<long, LeadTrackingCache> cachedLeads)
    {
        var leadsByCountry = groups.Select(y =>
        {
            long? score = null;
            if (cachedLeads.ContainsKey(y.LeadId!.Value))
                score = cachedLeads[y.LeadId.Value].Score;

            return new LeadItemCache
            {
                LeadId = y.LeadId,
                QueueId = y.LeadQueueId,
                Score = score
            };
        }).ToList();

        return leadsByCountry;
    }

    private IEnumerable<LeadStatusTypes>? DeserializeToSettings(string? jsonSettings, long clientId)
    {
        if (string.IsNullOrEmpty(jsonSettings))
        {
            _logger.LogWarning("Empty value in {ServiceType} ClientId: {clientId}",
                nameof(SettingTypes.LeadStatistic), clientId);
            return null;
        }

        var settings = JsonSerializer.Deserialize<LeadStatisticSettings>(jsonSettings, JsonSettingsExtensions.Default);

        if (settings is null)
        {
            _logger.LogWarning("Wrong value in {ServiceType} ClientId: {clientId}",
                nameof(SettingTypes.LeadStatistic), clientId);
            return null;
        }

        return settings.LeadStatuses;
    }
}
