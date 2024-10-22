using System.Text.Json;
using KL.Provider.Leads.Application.Configurations;
using KL.Provider.Leads.Application.Enums;
using KL.Provider.Leads.Application.Extensions;
using KL.Provider.Leads.Application.Models;
using KL.Provider.Leads.Application.Models.Messages;
using KL.Provider.Leads.Application.Services.Interfaces;
using KL.Provider.Leads.Persistent.Entities;
using KL.Provider.Leads.Persistent.LeadProviderHttpClient;
using KL.Provider.Leads.Persistent.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using Plat4me.Core.Nats;

namespace KL.Provider.Leads.Application.Services;

public class LeadsDataSourceSync : ILeadsDataSourceSync
{
    private readonly ILeadProviderClient _leadProviderClient;
    private readonly IClientRepository _clientRepository;
    private readonly ILeadRepository _leadRepository;
    private readonly IDataSourceRepository _dataSourceRepository;
    private readonly ISettingsRepository _settingsRepository;
    private readonly ILeadDataMapperService _leadDataMapperService;
    private readonly ILogger<LeadsDataSourceSync> _logger;
    private readonly PubSubjects _pubSubjects;
    private readonly INatsPublisher _natPublisher;
    private readonly Lazy<IEnumerable<TimeZoneProjection>> _timeZones;


    public LeadsDataSourceSync(
        ILeadProviderClient leadProviderClient,
        ILeadRepository leadRepository,
        IClientRepository clientRepository,
        IDataSourceRepository dataSourceRepository,
        ILeadDataMapperService leadDataMapperService,
        ILogger<LeadsDataSourceSync> logger,
        IOptions<PubSubjects> natsSubjects,
        INatsPublisher natPublisher,
        ISettingsRepository settingsRepository,
        IDataSourceMapService dataSourceMapService)
    {
        _leadProviderClient = leadProviderClient;
        _leadRepository = leadRepository;
        _clientRepository = clientRepository;
        _dataSourceRepository = dataSourceRepository;
        _leadDataMapperService = leadDataMapperService;
        _logger = logger;
        _pubSubjects = natsSubjects.Value;
        _natPublisher = natPublisher;
        _settingsRepository = settingsRepository;
        _timeZones = new Lazy<IEnumerable<TimeZoneProjection>>(dataSourceMapService.GetTimeZones);
    }

    public async Task LeadsSync(CancellationToken ct = default)
    {
        var clients = await _clientRepository.GetClients(ct);
        foreach (var client in clients)
        {
            await LeadsSyncForClient(client, ct);
        }

        var message = new LeadsImportedMessage(clients.Select(x => x.Id));
        await _natPublisher.PublishAsync(_pubSubjects.LeadsImported, message);
        _logger.LogInformation("Publish leads imported message with client ids: {message}",
            string.Join(", ", message.ClientIds));
    }

    private async Task LeadsSyncForClient(Client client, CancellationToken ct = default)
    {
        foreach (var dataSource in client.DataSources)
        {
            await LeadsSyncAndDataMapping(client, dataSource, ct);
        }
    }

    private async Task LeadsSyncAndDataMapping(
        Client client,
        DataSource dataSource,
        CancellationToken ct = default)
    {
        var settings = await GetLeadImportedSettings(client, ct);

        int leadsCount;
        const int pageSize = 1000;
        var page = 0;
        do
        {
            var dataSourceLeads = await _leadProviderClient.GetLeads(dataSource, page, ct);

            var dsLeads = dataSourceLeads
                .SelectMany(r => r.Value!.AsArray())
                .Select(lead => _leadDataMapperService.MapToLead(client.Id, dataSource.Id, settings, lead)!)
                .Where(x => x is not null)
                .ToArray();

            if (dsLeads.Length == 0) break;

            leadsCount = dsLeads.Length;

            var leadExternalIds = dsLeads
                .Where(i => !string.IsNullOrWhiteSpace(i.ExternalId))
                .Select(p => p.ExternalId);

            var existLeads = await _leadRepository.GetLeads(leadExternalIds, ct);
            var existExternalIds = existLeads.Select(i => i.ExternalId);

            var updateLeads = dsLeads.Where(i => existExternalIds.Contains(i.ExternalId));
            var insertLeads = dsLeads.Where(i => !existExternalIds.Contains(i.ExternalId)).ToArray();

            var leadPhones = dsLeads
                .Where(i => !string.IsNullOrWhiteSpace(i.Phone))
                .Select(p => p.Phone);
            _logger.LogWarning("Imported data source leads without phones: {leadsWithoutPhones}",
                string.Join(", ", dsLeads.Where(l => string.IsNullOrWhiteSpace(l.Phone)).Select(l => l.ExternalId)));

            var existsLeadsByPhone = await _leadRepository.GetLeadsByPhone(dataSource.Id, leadPhones, ct);

            CheckAndMarkOfDuplicate(insertLeads, existsLeadsByPhone);
            foreach (var item in insertLeads)
            {
                item.SystemStatus = LeadSystemStatusTypes.Imported;
                item.ImportedOn = DateTimeOffset.UtcNow;
                item.LastUpdateTime = DateTimeOffset.UtcNow;
                if (string.IsNullOrEmpty(item.Timezone))
                    item.Timezone = FetchZoneId(item.City, item.CountryCode);
            }

            await _leadRepository.MarkForSave(insertLeads, ct);

            UpdateLeadEntity(updateLeads, existLeads);
            await _leadRepository.SaveChanges(ct);
            page++;
        } while (leadsCount >= pageSize);

        await _dataSourceRepository.DataSourceUpdateDate(dataSource.Id, ct);
    }

    private async Task<LeadImportDefaultStatusSettings> GetLeadImportedSettings(Client client, CancellationToken ct)
    {
        var settingsJson = await _settingsRepository.GetValue(SettingTypes.LeadImportDefaultStatus, client.Id, ct);
        if (string.IsNullOrWhiteSpace(settingsJson))
            throw new ArgumentNullException(nameof(settingsJson),
                $"The {nameof(SettingTypes.LeadImportDefaultStatus)} JSON cannot be null");
        var settings =
            JsonSerializer.Deserialize<LeadImportDefaultStatusSettings>(settingsJson, JsonSettingsExtensions.Default);
        if (settings is null)
            throw new ArgumentNullException(nameof(settings),
                $"The {nameof(LeadImportDefaultStatusSettings)} settings cannot be null");

        return settings;
    }

    private static void CheckAndMarkOfDuplicate(
        IReadOnlyCollection<Lead> insertLeads,
        ICollection<Lead> existsLeadsByPhone)
    {
        var dicLeadsByPhone = existsLeadsByPhone.Where(i => i.DuplicateOfId is null).GroupBy(x => x.Phone)
            .SelectMany(x => x.OrderBy(i => i.RegistrationTime).Take(1))
            .ToDictionary(r => r.Phone, r => r);

        var groupByPhoneLeads = insertLeads.GroupBy(i => i.Phone);
        foreach (var leadGroup in groupByPhoneLeads)
        {
            if (leadGroup.Count() > 1)
            {
                var originateLead = leadGroup.OrderBy(r => r.RegistrationTime).First();
                LeadGroupsMarkOfDuplicate(dicLeadsByPhone, originateLead, leadGroup);
            }
            else if (dicLeadsByPhone.ContainsKey(leadGroup.Key))
            {
                var duplicate = leadGroup.First();
                duplicate.DuplicateOfId = dicLeadsByPhone[duplicate.Phone].Id;
            }
        }
    }

    private static void LeadGroupsMarkOfDuplicate(
        IReadOnlyDictionary<string, Lead> existPhones,
        Lead originateLead,
        IGrouping<string, Lead> leadGroup)
    {
        if (existPhones.ContainsKey(originateLead.Phone))
        {
            foreach (var leadItem in leadGroup)
            {
                leadItem.DuplicateOfId = existPhones[originateLead.Phone].Id;
            }
        }
        else
        {
            var duplicateLids = leadGroup.Except(new[] { originateLead });
            foreach (var leadItem in duplicateLids)
                originateLead.Duplicates.Add(leadItem);
        }
    }

    private void UpdateLeadEntity(
        IEnumerable<Lead> updateLeads,
        IReadOnlyCollection<Lead> dbExistLeads)
    {
        foreach (var lead in updateLeads)
        {
            if (string.IsNullOrEmpty(lead.Timezone))
                lead.Timezone = FetchZoneId(lead.City, lead.CountryCode);
            var dbLead = dbExistLeads.First(i => i.ExternalId == lead.ExternalId);
            _leadRepository.UpdateLead(lead, dbLead.Id);

            if (dbLead.Phone != lead.Phone)
            {
                _logger.LogError("The incoming phone number {0} is different for db lead phone: {1}", lead.Phone,
                    dbLead.Phone);
            }
        }
    }

    public string? FetchZoneId(string? city, string? countryCode)
    {
        string? timeZone = null;
        if (!string.IsNullOrEmpty(city))
            timeZone = _timeZones.Value.FirstOrDefault(t => t.CityName == city)?.Timezone;

        if (!string.IsNullOrEmpty(countryCode) && string.IsNullOrEmpty(timeZone))
            timeZone = _timeZones.Value.FirstOrDefault(t => t.CountryCode == countryCode)?.Timezone;

        return timeZone;
    }
}