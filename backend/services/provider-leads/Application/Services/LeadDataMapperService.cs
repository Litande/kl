using System.Text.Json.Nodes;
using KL.Provider.Leads.Application.Enums;
using KL.Provider.Leads.Application.Models;
using KL.Provider.Leads.Application.Services.Interfaces;
using KL.Provider.Leads.Persistent.Entities;

namespace KL.Provider.Leads.Application.Services;

public class LeadDataMapperService : ILeadDataMapperService
{
    private readonly ILogger<LeadDataMapperService> _logger;

    private readonly Lazy<IEnumerable<LeadDataSourceMap>> _leadPropertiesContractMapper;
    private readonly Lazy<IEnumerable<UserDataSourceMap>> _userDataSourceMap;
    private readonly Lazy<IEnumerable<StatusDataSourceMap>> _statusDataSourceMap;

    public LeadDataMapperService(
        ILogger<LeadDataMapperService> logger,
        IDataSourceMapService dataSourceMapService)
    {
        _logger = logger;

        _leadPropertiesContractMapper =
            new Lazy<IEnumerable<LeadDataSourceMap>>(dataSourceMapService.GetLeadDataSources);
        _userDataSourceMap = new Lazy<IEnumerable<UserDataSourceMap>>(dataSourceMapService.GetUserDataSources);
        _statusDataSourceMap = new Lazy<IEnumerable<StatusDataSourceMap>>(dataSourceMapService.GetStatusDataSources);
    }

    public Lead? MapToLead(
        long clientId,
        long dataSourceId,
        LeadImportDefaultStatusSettings settings,
        JsonNode? lead)
    {
        var entityLead = new Lead();
        var leadPropertiesContractMapper = LeadPropertiesContractMapper(dataSourceId);
        var userDataSourceMap = UserDataSourceMap(dataSourceId);
        var statusDataSourceMap = StatusDataSourceMap(dataSourceId);
       
        if (!TryGetLeadStatus(lead, leadPropertiesContractMapper, statusDataSourceMap, out var statusId))
        {
            _logger.LogWarning("Not found LeadStatus mapping for status {StatusId}", statusId);
            return null;
        }

        foreach (var leadData in leadPropertiesContractMapper)
        {
            entityLead.ClientId = clientId;
            entityLead.DataSourceId = dataSourceId;
            entityLead.Metadata = lead?.ToJsonString();

            var key = leadData.Key;
            var jsonNode = lead?[key];

            SetLeadProperty(
                leadData,
                statusDataSourceMap,
                userDataSourceMap,
                entityLead,
                jsonNode);
        }

        return entityLead;
    }

    private static bool TryGetLeadStatus(JsonNode? leadJsonNode,
        IReadOnlyDictionary<string, LeadDataSource> leadPropertiesContractMapper,
        IReadOnlyDictionary<string, LeadStatusTypes> statusDataSourceMap,
        out string statusId)
    {
        var key = leadPropertiesContractMapper.FirstOrDefault(x => x.Value == LeadDataSource.StatusId).Key;
        var jsonNode = leadJsonNode?[key];
        statusId = jsonNode!.ToString();
        return statusDataSourceMap.ContainsKey(jsonNode.ToJsonString());
    }

    private IReadOnlyDictionary<string, LeadStatusTypes> StatusDataSourceMap(long dataSourceId)
    {
        return _statusDataSourceMap.Value.Where(i => i.DataSourceId == dataSourceId)
            .ToDictionary(i => i.ExternalStatusId, i => i.Status);
    }

    private IReadOnlyDictionary<long, long> UserDataSourceMap(long dataSourceId)
    {
        return _userDataSourceMap.Value.Where(i => i.DataSourceId == dataSourceId)
            .ToDictionary(i => i.UserId, i => i.EmployeeId);
    }

    private IReadOnlyDictionary<string, LeadDataSource> LeadPropertiesContractMapper(long dataSourceId)
    {
        return _leadPropertiesContractMapper.Value
            .Where(i => i.DataSourceId == dataSourceId)
            .ToDictionary(i => i.SourceProperty, i => i.DestinationProperty);
    }

    private void SetLeadProperty(
        KeyValuePair<string, LeadDataSource> mapperLeadContract,
        IReadOnlyDictionary<string, LeadStatusTypes> statusDataSourceMap,
        IReadOnlyDictionary<long, long> userDataSourceMap,
        Lead entityLead,
        JsonNode? jsonNode)
    {
        switch (mapperLeadContract.Value)
        {
            case LeadDataSource.Phone:
                var phone = jsonNode?.ToString();
                if (string.IsNullOrWhiteSpace(phone))
                {
                    _logger.LogError("Lead phone cannot be null");
                    break;
                }

                entityLead.Phone = phone;
                break;
            case LeadDataSource.FirstName:
                entityLead.FirstName = jsonNode?.ToString() ?? string.Empty;
                break;
            case LeadDataSource.LastName:
                entityLead.LastName = jsonNode?.ToString() ?? string.Empty;
                break;
            case LeadDataSource.CountryCode:
                entityLead.CountryCode = jsonNode?.ToString();
                break;
            case LeadDataSource.LanguageCode:
                entityLead.LanguageCode = jsonNode?.ToString();
                break;
            case LeadDataSource.ExternalId:
                entityLead.ExternalId = jsonNode?.ToString();
                break;
            case LeadDataSource.StatusId:
                entityLead.Status = statusDataSourceMap[jsonNode!.ToJsonString()];
                break;
            case LeadDataSource.LastTimeOnline:
                entityLead.LastTimeOnline = (DateTimeOffset?)jsonNode;
                break;
            case LeadDataSource.RegistrationTime:
                entityLead.RegistrationTime = (DateTimeOffset)(jsonNode ?? DateTimeOffset.UtcNow);
                break;
            case LeadDataSource.FirstDepositTime:
                entityLead.FirstDepositTime = (DateTimeOffset?)jsonNode;
                break;
            case LeadDataSource.AssignedUserId:
                var externalUserId = (long?)jsonNode;
                if (!externalUserId.HasValue) break;
                var hasMappedAssignedUser = userDataSourceMap.ContainsKey(externalUserId.Value);
                if (!hasMappedAssignedUser)
                    _logger.LogError("Missing user data source mapping for: {0}", externalUserId.Value);
                else
                    entityLead.AssignedUserId = userDataSourceMap[externalUserId.Value];
                break;
            case LeadDataSource.LastUpdateTime:
                break;
            case LeadDataSource.Timezone:
                entityLead.Timezone = jsonNode?.ToString();
                break;
            case LeadDataSource.City:
                entityLead.City = jsonNode?.ToString();
                break;
            case LeadDataSource.Email:
                entityLead.Email = jsonNode?.ToString();
                break;
            default:
                _logger.LogWarning("Lead property type {Type} mapper not implemented", mapperLeadContract.Value);
                break;
        }
    }
}