using System.Web;
using KL.Nats;
using KL.SIP.Bridge.Application.Configurations;
using KL.SIP.Bridge.Application.Connections;
using KL.SIP.Bridge.Application.Enums;
using KL.SIP.Bridge.Application.Extensions;
using KL.SIP.Bridge.Application.Models;
using KL.SIP.Bridge.Application.Models.Messages;
using KL.SIP.Bridge.Application.Services;
using Microsoft.Extensions.Options;
using SIPSorcery.Net;
using SIPSorcery.SIP;

namespace KL.SIP.Bridge.Application.Session;

public class CallSessionBase : ICallSession
{
    public string Id { get; protected set; }
    public InitCallData CallData { get; protected set; } = null!;

    public event Action<ICallSession>? OnClosed;

    protected IServiceScope _callScope;

    protected long _sessionTimeoutValue;
    protected Timer? _sessionTimeout;

    protected IConnection? _agentConnection;
    protected IConnection? _leadConnection;
    protected IConnection? _managerConnection;
    protected ManagerConnectionMode _managerMode = ManagerConnectionMode.ListenOnly;

    protected DateTimeOffset _originatedAt;
    protected DateTimeOffset? _leadAnswerAt;
    protected DateTimeOffset? _agentAnswerAt;
    protected IAudioStreamRecorder? _leadRecorder;
    protected IAudioStreamRecorder? _agentRecorder;
    protected IAudioStreamRecorder? _managerRecorder;

    protected ILogger<ICallSession> _logger;
    protected readonly string _accessUrl;
    protected readonly INatsPublisher _natsPublisher;
    protected readonly NatsSubjects _natsSubjects;
    protected readonly GeneralOptions _generalOptions;
    protected readonly ILeadConnectionFactory _leadConnectionFactory;

    protected ISessionRecordingService _sessionRecordingService;

    protected const int ConnectionMainStream = 0;
    protected const int ManagerConnectionLeadStreamIdx = 0;
    protected const int ManagerConnectionAgentStreamIdx = 1;
    protected const int ManagerAudioStreamIdx = 1;

    protected string _rtcUrl = null!;
    protected bool _isClosed = false;
    protected bool _agentDropped = false;
    protected long? _connectedManagerId;

    private readonly SIPTransport _transport;

    public CallSessionBase(
        IServiceScope callScope,
        ILogger<ICallSession> logger,
        string accessUrl,
        long sessionTimeoutValue,
        IOptions<NatsSubjects> natsSubjects,
        IOptions<GeneralOptions> generalOptions,
        ILeadConnectionFactory leadConnectionFactory,
        INatsPublisher natsPublisher,
        ISessionRecordingService sessionRecordingService,
        string sessionId,
        SIPTransport transport
    )
    {
        _callScope = callScope;
        _logger = logger;
        _accessUrl = accessUrl;
        _sessionTimeoutValue = sessionTimeoutValue;
        _natsSubjects = natsSubjects.Value;
        _natsPublisher = natsPublisher;
        _generalOptions = generalOptions.Value;
        _leadConnectionFactory = leadConnectionFactory;
        _sessionRecordingService = sessionRecordingService;

        Id = sessionId;
        _transport = transport;
    }

    public virtual async Task Start(InitCallData initCallData)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(initCallData.LeadPhone))
                throw new ArgumentException("Invalid called number");

            CallData = initCallData;
            _originatedAt = DateTimeOffset.UtcNow;

            PrepareRtcUrls();
            _sessionRecordingService.Init(Id, initCallData.LeadPhone);
            _sessionTimeout = new Timer(
                SessionTimeout,
                null,
                _sessionTimeoutValue,
                Timeout.Infinite);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while session starting");
            await Close(new CloseCommand(CallFinishReasons.Unknown));
        }
    }

    protected async void SessionTimeout(object? state)
    {
        await Close(new CloseCommand(CallFinishReasons.SessionTimeout));
    }

    public virtual async Task Close(CloseCommand cmd)
    {
        if (_isClosed) return;
        _isClosed = true;
        OnClosed?.Invoke(this);

        var finishedMessage = new CallFinishedMessage(
                    CallData.ClientId,
                    _generalOptions.InstanceId,
                    Id,
                    CallData.CallType,
                    CallData.AgentId,
                    CallData.QueueId,
                    CallData.LeadId,
                    CallData.LeadPhone,
                    cmd.ReasonCode,
                    cmd.ReasonDetails,
                    CallData.SipProvider.Id,
                    _leadConnection?.LastErrorCode,
                    _originatedAt,
                    _leadAnswerAt,
                    _agentAnswerAt,
                    DateTimeOffset.UtcNow,
                    cmd.AgentComment,
                    cmd.ManagerComment,
                    _agentDropped
                );

        if (cmd.ReasonCode.IsSuccessFinish())
        {
            _logger.LogDebug(
                "CallFinishedMessage Session={sessionId}, AgentId={agentId}, LeadId={leadId}, LeadPhone={leadPhone}, HangupCause={reasonCode}({reason})",
                Id, CallData.AgentId, CallData.LeadId, CallData.LeadPhone, cmd.ReasonCode, cmd.ReasonDetails);

            await _natsPublisher.PublishAsync(_natsSubjects.CallFinished,
                finishedMessage);
        }
        else
        {
            _logger.LogDebug(
                "CallFailedMessage Session={sessionId}, AgentId={agentId}, LeadId={leadId}, LeadPhone={leadPhone}, HangupCause={reasonCode}({reason})",
                Id, CallData.AgentId, CallData.LeadId, CallData.LeadPhone, cmd.ReasonCode, cmd.ReasonDetails);

            await _natsPublisher.PublishAsync(_natsSubjects.CallFailed,
                finishedMessage);
        }

        CleanupLeadConnection();
        CleanupAgentConnection();
        CleanupManagerConnection();

        await _sessionRecordingService.WaitAllRecordings();

        _callScope.Dispose();
    }

    public async Task ReplaceAgent(long? agentId)
    {
        if (agentId is null)
        {
            await Close(new CloseCommand(CallFinishReasons.NoAvailableAgents));
        }
        else
        {
            CallData = CallData with { AgentId = agentId.Value };
            CleanupAgentConnection();
            PrepareRtcUrls();
            await InviteAgent();
        }
    }

    public async Task<bool> SetAgentConnection(IConnection connection, long agentId)
    {
        if (agentId != CallData.AgentId || _agentDropped)
        {
            connection.Hangup();
            connection.Dispose();
            return false;
        }
        else
        {
            CleanupAgentConnection();//Force close, attended transfer?
            _agentConnection = connection;
            _agentConnection.OnCallHangup += CallHangupAgent;
            _agentConnection.OnCallFailed += CallFailed;//RTCConnection: generated only on timeout
            //_agentConnection.OnCallAnswered += this.AgentAnswer;//RTCConnection: not generated
            //_agentConnection.Start();//RTCConnection: do nothing
            await AgentAnswer();
            return true;
        }
    }

    protected void CleanupLeadConnection()
    {
        if (_leadConnection is null)
            return;

        var conn = _leadConnection;
        _leadConnection = null;

        conn.OnData -= LeadSessionBridge;
        conn.OnCallFailed -= CallFailed;
        conn.OnCallHangup -= CallHangupLead;
        conn.OnCallRinging -= LeadRinging;

        conn.Hangup();
        conn.Dispose();
        _sessionRecordingService.StopRecording(_leadRecorder);
        _leadRecorder = null;
    }

    protected void CleanupAgentConnection()
    {
        if (_agentConnection is null)
            return;

        var conn = _agentConnection;
        _agentConnection = null;
        _leadConnection?.SendToAudioStream(ConnectionMainStream, null!);

        conn.OnCallHangup -= CallHangupAgent;
        conn.OnCallFailed -= CallFailed;
        conn.OnData -= AgentSessionBridge;

        conn.Hangup();
        conn.Dispose();
        _sessionRecordingService.StopRecording(_agentRecorder);
        _agentRecorder = null;
    }

    public virtual void OnAgentConnectionLost()
    {
        if (_agentDropped) return;
        CleanupAgentConnection();

        //TODO send notification?
    }

    protected async void CallHangupLead()
    {
        var cmd = new CloseCommand(
            _agentConnection is null ? CallFinishReasons.AgentNotAnswerLeadHangUp : CallFinishReasons.CallFinishedByLead
        );
        await Close(cmd);
    }

    protected async void CallHangupAgent()
    {
        var cmd = new CloseCommand(CallFinishReasons.CallFinishedByAgent);
        await Close(cmd);
    }

    protected async void CallFailed(CallFinishReasons reason)
    {
        var cmd = new CloseCommand(reason);
        await Close(cmd);
    }

    protected virtual async Task LeadAnswer()
    {
        _logger.LogInformation("Invite agent {AgentId} to session {Id}", CallData.AgentId, Id);
        _leadAnswerAt = DateTimeOffset.UtcNow;

        var msg = new CalleeAnsweredMessage(
            CallData.ClientId,
            _generalOptions.InstanceId,
            Id,
            CallData.CallType,
            _agentAnswerAt,
            _leadAnswerAt,
            _originatedAt,
            CallData.AgentId,
            CallData.QueueId,
            CallData.LeadId,
            CallData.LeadPhone,
            _rtcUrl,
            _rtcUrl,
            CallData.SipProvider.Id
        );
        _logger.LogDebug("LeadAnswer Session={sessionId}, AgentId={agentId}, LeadId={leadId}, LeadPhone={leadPhone}, RTC={RTCUrl}",
            Id, CallData.AgentId, CallData.LeadId, CallData.LeadPhone, _rtcUrl);
        await _natsPublisher.PublishAsync(_natsSubjects.LeadAnswered, msg);

        _leadRecorder = await _sessionRecordingService.CreateLeadRecorder();
        _leadConnection!.OnData += LeadSessionBridge;
    }

    protected Task InviteLead()
    {
        _leadConnection = _leadConnectionFactory.CreateLeadConnection(CallData, _transport);
        _leadConnection.OnCallAnswered += async () => { await LeadAnswer(); };
        _leadConnection.OnCallFailed += CallFailed;
        _leadConnection.OnCallHangup += CallHangupLead;
        _leadConnection.OnCallRinging += LeadRinging;
        _ = _leadConnection.Start();
        return Task.CompletedTask;
    }

    protected virtual void LeadRinging()
    {

    }

    protected async Task InviteAgent()
    {
        _logger.LogDebug("InviteAgentMessage Session={sessionId}, AgentId={agentId}, LeadId={leadId}, Url={url}",
            Id, CallData.AgentId, CallData.LeadId, _rtcUrl);

        await _natsPublisher.PublishAsync(_natsSubjects.InviteAgent,
            new InviteAgentMessage(
                CallData.ClientId,
                _generalOptions.InstanceId,
                Id,
                CallData.CallType,
                CallData.AgentId,
                CallData.LeadId,
                CallData.LeadPhone,
                _originatedAt,
                _leadAnswerAt,
                _rtcUrl,
                CallData.SipProvider.Id
            ));
    }

    protected virtual async Task AgentAnswer()
    {
        if (_agentAnswerAt is null)
        {
            _agentAnswerAt = DateTimeOffset.UtcNow;
        }

        var msg = new CalleeAnsweredMessage(
            CallData.ClientId,
            _generalOptions.InstanceId,
            Id,
            CallData.CallType,
            _agentAnswerAt.Value,
            _leadAnswerAt,
            _originatedAt,
            CallData.AgentId,
            CallData.QueueId,
            CallData.LeadId,
            CallData.LeadPhone,
            _rtcUrl,
            _rtcUrl,
            CallData.SipProvider.Id
        );
        _logger.LogDebug("AgentAnswered Session={sessionId}, AgentId={agentId}, LeadId={leadId}, LeadPhone={leadPhone}, RTC={RtcUrl}",
            Id, CallData.AgentId, CallData.LeadId, CallData.LeadPhone, _rtcUrl);
        await _natsPublisher.PublishAsync(_natsSubjects.AgentAnswered, msg);

        _agentRecorder = await _sessionRecordingService.CreateAgentRecorder();
        try
        {
            _agentConnection!.OnData += AgentSessionBridge;
        }
        catch (Exception ex) //connection could be closed at this point
        {
            _logger.LogError(ex, "Agent missing");
        }
    }

    protected virtual bool DropAgent(string? reason = null)
    {
        if (_managerMode == ManagerConnectionMode.BothDirections)
        {
            _agentDropped = true;
            CleanupAgentConnection();
            _natsPublisher.PublishAsync(_natsSubjects.DroppedAgent, new DroppedAgentMessage(
                  CallData.ClientId,
                    _generalOptions.InstanceId,
                    Id,
                    CallData.CallType,
                    CallData.QueueId,
                    CallData.AgentId,
                    CallData.LeadId,
                    CallData.IsFixedAssigned,
                    CallData.LeadPhone,
                    CallData.SipProvider.Id,
                    DateTimeOffset.UtcNow,
                    _connectedManagerId ?? 0,
                    reason
            ));
            return true;
        }
        return false;
    }

    public virtual async Task ProcessCommand(AgentCommand agentCommand)
    {
        switch (agentCommand.Command)
        {
            case AgentCommandTypes.HangupCall:
                {
                    string? hangupReason = null;
                    string? comment = null;
                    agentCommand.Params?.TryGetValue("hangupReason", out hangupReason);
                    agentCommand.Params?.TryGetValue("comment", out comment);
                    var cmd = new CloseCommand(CallFinishReasons.CallFinishedByAgent)
                    {
                        ReasonDetails = hangupReason,
                        AgentComment = comment
                    };
                    await Close(cmd);
                    break;
                }
            default:
                _logger.LogWarning("Unknown agent command. Ignore.");
                break;
        }
    }

    public virtual async Task ProcessCommand(ManagerCommand managerCommand)
    {
        switch (managerCommand.Command)
        {
            case ManagerCommandTypes.HangupCall:
                {
                    string? hangupReason = null;
                    string? comment = null;
                    managerCommand.Params?.TryGetValue("hangupReason", out hangupReason);
                    managerCommand.Params?.TryGetValue("comment", out comment);
                    var cmd = new CloseCommand(CallFinishReasons.CallFinishedByManager)
                    {
                        ReasonDetails = hangupReason,
                        ManagerComment = comment
                    };
                    await Close(cmd);
                    break;
                }
            case ManagerCommandTypes.SetManagerMode:
                {
                    if (managerCommand.Params is not null
                        && managerCommand.Params.TryGetValue("mode", out var modeStr)
                        && Enum.TryParse<ManagerConnectionMode>(modeStr, true, out var mode))
                    {
                        SetManagerMode(mode);
                    }
                    else
                        _logger.LogError("Can not set manager mode - value is missing or invalid");
                    break;
                }
            case ManagerCommandTypes.DropAgent:
                {
                    string? comment = null;
                    managerCommand.Params?.TryGetValue("comment", out comment);
                    DropAgent();
                    break;
                }
            default:
                _logger.LogWarning("Unknown agent command. Ignore.");
                break;
        }
    }

    protected void LeadSessionBridge(IConnection conn, SDPMediaTypesEnum mediaType, RTPPacket packet)
    {
        if (mediaType != SDPMediaTypesEnum.audio)
            return;

        _agentConnection?.SendToAudioStream(ConnectionMainStream, packet);
        _managerConnection?.SendToAudioStream(ManagerConnectionLeadStreamIdx, packet);
        _leadRecorder?.EnqueuePacket(packet);
    }

    protected void AgentSessionBridge(IConnection conn, SDPMediaTypesEnum mediaType, RTPPacket packet)
    {
        if (mediaType != SDPMediaTypesEnum.audio)
            return;

        _leadConnection?.SendToAudioStream(ConnectionMainStream, packet);
        _managerConnection?.SendToAudioStream(ManagerConnectionAgentStreamIdx, packet);
        _agentRecorder?.EnqueuePacket(packet);
    }

    protected void ManagerSessionBridge(IConnection conn, SDPMediaTypesEnum mediaType, RTPPacket packet)
    {
        if (mediaType != SDPMediaTypesEnum.audio)
            return;
        //TODO test manager audio recording
        switch (_managerMode)
        {
            case ManagerConnectionMode.BothDirections:
                _leadConnection?.SendToAudioStream(ManagerAudioStreamIdx, packet);
                _agentConnection?.SendToAudioStream(ManagerAudioStreamIdx, packet);
                _managerRecorder?.EnqueuePacket(packet);
                break;
            case ManagerConnectionMode.AgentOnly:
                _agentConnection?.SendToAudioStream(ManagerAudioStreamIdx, packet);
                _managerRecorder?.EnqueuePacket(packet);
                break;
            case ManagerConnectionMode.ListenOnly:
            default:
                _managerRecorder?.EnqueuePacket(new RTPPacket() { Payload = IAudioStreamRecorder.SilenceData, Header = packet.Header });
                break;
        }
    }

    private void SetManagerMode(ManagerConnectionMode mode)
    {
        if (_agentDropped)
        {
            _logger.LogInformation("Cannot change manager mode to {mode} on session {id}. Agent dropped.", mode, Id);
            return;
        }
        _managerMode = mode;
        if (_managerMode != ManagerConnectionMode.BothDirections)
            _leadConnection?.SendToAudioStream(ManagerAudioStreamIdx, null!);
        _logger.LogInformation("Set manager mode {mode} on session {id}", mode, Id);
    }

    public async Task AddManagerConnection(IConnection connection, long userId)
    {
        _logger.LogInformation("Manager connected to session {id}", Id);
        if (_managerConnection is not null)
        {
            CleanupManagerConnection();
        }

        if (_leadConnection is not null)
        {
            _managerConnection = connection;
            _managerConnection.OnCallFailed += CallFailedManager;
            _managerConnection.OnCallHangup += CallHangupManager;
            _managerConnection.OnData += ManagerSessionBridge;
            _managerRecorder = await _sessionRecordingService.CreateManagerRecorder();
            _connectedManagerId = userId;
        }
        else
        {
            connection.Hangup();
            connection.Dispose();
        }
    }

    protected void CallFailedManager(CallFinishReasons cause)
    {
        _logger.LogInformation("Manager connection failed with {cause}", cause);
        CleanupManagerConnection();
    }

    protected async void CallHangupManager()
    {
        CleanupManagerConnection();
        if (_agentDropped)
            await Close(new CloseCommand(CallFinishReasons.CallFinishedByManager));
    }

    protected void CleanupManagerConnection()
    {
        _managerMode = ManagerConnectionMode.ListenOnly;

        if (_managerConnection is null)
            return;

        var conn = _managerConnection;
        _managerConnection = null;
        _connectedManagerId = null;
        conn.OnCallFailed -= CallFailedManager;
        conn.OnCallHangup -= CallHangupManager;
        conn.OnData -= ManagerSessionBridge;
        conn.Hangup();
        conn.Dispose();
        _sessionRecordingService.StopRecording(_managerRecorder);
    }

    protected void PrepareRtcUrls()
    {
        var url = new UriBuilder(_accessUrl!);
        var httpValueCollection = HttpUtility.ParseQueryString(url.Query);
        httpValueCollection.Remove("session");
        httpValueCollection.Add("session", Id);
        url.Query = httpValueCollection.ToString();
        _rtcUrl = url.ToString();
    }
}
