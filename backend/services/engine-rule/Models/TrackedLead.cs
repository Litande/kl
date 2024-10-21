using Plat4Me.DialRuleEngine.Application.Enums;
using Plat4Me.DialRuleEngine.Application.Models.Entities;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Plat4Me.DialRuleEngine.Application.Extensions;

namespace Plat4Me.DialRuleEngine.Application.Models;

public class TrackedLead
{
    protected enum TrackedLeadFields
    {
        FirstName = 1,
        LastName = 2,
        LeadPhone = 3,
        RegistrationTime = 4,
        CountryCode = 5,
        RemindOn = 6,
        LastCallAgentId = 7,
        DeletedOn = 8,
        //TimeZone = 9,
        FreezeTo = 11,
        AssignAgentId = 12,
    }

    protected Dictionary<TrackedLeadFields, List<ValueChanges<object?>>> _dataChanges = new();
    protected List<ValueChanges<object?>> _statusChanges = new();
    protected List<KeyValuePair<DateTimeOffset, LeadStatusTypes>> _statusHistory { get; set; } = new();

    protected long _leadId;
    protected string _firstName = null!;
    protected string _lastName = null!;
    protected string _leadPhone = null!;
    protected LeadStatusTypes _status;
    protected DateTimeOffset _registrationTime;
    protected DateTimeOffset? _remindOn;
    protected long? _lastCallAgentId;
    protected long? _assignedAgentId;
    protected string? _countryCode;
    protected DateTimeOffset? _deletedOn;
    protected LeadSystemStatusTypes? _systemStatus;
    protected string? _timezone;
    protected bool _isTest;
    protected DateTimeOffset? _freezeTo;
    protected string? _metaData;
    protected JsonObject? _metaDataObject;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
    };

    public TrackedLead(
        long leadId,
        string firstName,
        string lastName,
        string leadPhone,
        LeadStatusTypes status,
        LeadSystemStatusTypes? systemStatus,
        DateTimeOffset registrationTime,
        DateTimeOffset? remindOn,
        long? lastCallAgentId,
        string? countryCode,
        DateTimeOffset? deletedOn,
        string? timezone,
        bool isTest,
        long? assignedAgentId,
        DateTimeOffset? freezeTo,
        IEnumerable<KeyValuePair<DateTimeOffset, string>>? statusHistory,
        string? metaData
    )
    {
        _leadId = leadId;
        _firstName = firstName;
        _lastName = lastName;
        _leadPhone = leadPhone;
        _status = status;
        _systemStatus = systemStatus;
        _registrationTime = registrationTime;
        _remindOn = remindOn;
        _lastCallAgentId = lastCallAgentId;
        _countryCode = countryCode;
        _deletedOn = deletedOn;
        _timezone = timezone;
        _isTest = isTest;
        _assignedAgentId = assignedAgentId;
        _freezeTo = freezeTo;
        _metaData = metaData;
        ParseHistory(statusHistory);
    }

    public long LeadId => _leadId;
    public string? Timezone => _timezone;
    public bool IsTest => _isTest;
    public JsonObject? MetaData
    {
        get
        {
            if (string.IsNullOrEmpty(_metaData)) return null;
            if (_metaDataObject is null)
                _metaDataObject = JsonSerializer.Deserialize<JsonObject>(_metaData, _jsonOptions);
            return _metaDataObject;
        }
    }

    public string FirstName
    {
        get => _firstName;
        set
        {
            TrackDataChange(TrackedLeadFields.FirstName, _firstName, value);
            _firstName = value;
        }
    }

    public string LastName
    {
        get => _lastName;
        set
        {
            TrackDataChange(TrackedLeadFields.LastName, _lastName, value);
            _lastName = value;
        }
    }

    public string LeadPhone
    {
        get => _leadPhone;
        set
        {
            TrackDataChange(TrackedLeadFields.LeadPhone, _leadPhone, value);
            _leadPhone = value;
        }
    }

    public LeadStatusTypes Status
    {
        get => _status;
        set
        {
            _statusChanges.Add(new ValueChanges<object?>("Status", _status, value));
            _status = value;
        }
    }

    public LeadSystemStatusTypes? SystemStatus => _systemStatus;


    public DateTimeOffset RegistrationTime
    {
        get => _registrationTime;
        set
        {
            TrackDataChange(TrackedLeadFields.RegistrationTime, _registrationTime, value);
            _registrationTime = value;
        }
    }

    public string? CountryCode
    {
        get => _countryCode;
        set
        {
            TrackDataChange(TrackedLeadFields.CountryCode, _countryCode, value);
            _countryCode = value;
        }
    }

    public DateTimeOffset? RemindOn
    {
        get => _remindOn;
        set
        {
            TrackDataChange(TrackedLeadFields.RemindOn, _remindOn, value);
            _remindOn = value;
        }
    }

    public long? LastCallAgentId
    {
        get => _lastCallAgentId;
        set
        {
            TrackDataChange(TrackedLeadFields.LastCallAgentId, _lastCallAgentId, value);
            _lastCallAgentId = value;
        }
    }

    public DateTimeOffset? DeletedOn
    {
        get => _deletedOn;
        set
        {
            TrackDataChange(TrackedLeadFields.DeletedOn, _deletedOn, value);
            _deletedOn = value;
        }
    }

    public long? AssignedAgentId
    {
        get => _assignedAgentId;
        set
        {
            TrackDataChange(TrackedLeadFields.AssignAgentId, _assignedAgentId, value);
            _assignedAgentId = value;
        }
    }

    public DateTimeOffset? FreezeTo
    {
        get => _freezeTo;
        set
        {
            TrackDataChange(TrackedLeadFields.FreezeTo, _freezeTo, value);
            _freezeTo = value;
        }
    }

    public long? LeadQueueId { get; set; }
    public long Score { get; set; }


    public IReadOnlyCollection<KeyValuePair<DateTimeOffset, LeadStatusTypes>> StatusHistory => _statusHistory;
    public double MinutesSinceRegistration => (DateTimeOffset.UtcNow - RegistrationTime).TotalMinutes;

    public IReadOnlyCollection<LeadHistory> GetTrackedChanges()
    {
        var result = new List<LeadHistory>();
        if (_statusChanges.Any())
        {
            var time = DateTimeOffset.UtcNow;
            result.Add(new LeadHistory
            {
                LeadId = _leadId,
                ActionType = LeadHistoryActionType.Status,
                Changes = JsonSerializer.Serialize(
                    new LeadHistoryChangesDto<object?>
                    {
                        Properties = _statusChanges
                        //,Version = "1.0"
                    },
                 _jsonOptions),
                CreatedAt = time,
                CreatedBy = null
            });
            _statusHistory.AddRange(_statusChanges.Select(x => new KeyValuePair<DateTimeOffset, LeadStatusTypes>(time, (LeadStatusTypes)x.To!)));
            _statusChanges.Clear();
        }

        if (_dataChanges.Any())
        {
            result.Add(new LeadHistory
            {
                LeadId = _leadId,
                ActionType = LeadHistoryActionType.Data,
                Changes = JsonSerializer.Serialize(
                    new LeadHistoryChangesDto<object?>
                    {
                        Properties = _dataChanges.SelectMany(x => x.Value).ToList()
                        //,Version = "1.0"
                    },
                    _jsonOptions),
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = null
            });
            _dataChanges.Clear();
        }

        return result;
    }

    private void TrackDataChange(TrackedLeadFields field, object? from, object? to)
    {
        if (!_dataChanges.ContainsKey(field))
        {
            _dataChanges.Add(field, new List<ValueChanges<object?>>());
        }

        _dataChanges[field].Add(new ValueChanges<object?>(field.ToString(), from, to));
    }

    private void ParseHistory(IEnumerable<KeyValuePair<DateTimeOffset, string>>? statusHistory)
    {
        var history = new List<KeyValuePair<DateTimeOffset, LeadStatusTypes>>();
        if (statusHistory is not null)
        {
            foreach (var (dateOn, changes) in statusHistory)
            {
                var changesDto = JsonSerializer.Deserialize<LeadHistoryChangesDto<LeadStatusTypes>>(changes, _jsonOptions);
                if (changesDto is null || changesDto.Properties is null || !changesDto.Properties.Any())
                    throw new ArgumentException("Invalid lead changes value");
                foreach (var statusChange in changesDto.Properties)
                {
                    history.Add(new KeyValuePair<DateTimeOffset, LeadStatusTypes>(dateOn, statusChange.To));
                }
            }
        }
        //_statusHistory = history.OrderBy(x => x.Key).ToList();
        _statusHistory = history;
    }
}
