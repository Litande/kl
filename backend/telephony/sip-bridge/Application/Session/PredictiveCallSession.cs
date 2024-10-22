using KL.Nats;
using KL.SIP.Bridge.Application.Configurations;
using KL.SIP.Bridge.Application.Enums;
using KL.SIP.Bridge.Application.Models.Messages;
using Microsoft.Extensions.Options;
using SIPSorcery.SIP;

namespace KL.SIP.Bridge.Application.Session;

public class PredictiveCallSession : CallSessionBase
{
    protected Timer? _agentTimeout;
    protected Timer? _callDurationTimeout;
    protected Timer? _ringingTimeout;
    protected long? _maxCallDuration;
    protected long? _maxRingingTime;
    protected PredictiveTimeouts _timeouts;

    public PredictiveCallSession
    (
        IServiceScope callScope,
        ILogger<ICallSession> logger,
        IOptions<CallSessionOptions> sessionOptions,
        IOptions<NatsSubjects> natsSubjects,
        IOptions<GeneralOptions> generalOptions,
        ILeadConnectionFactory leadConnectionFactory,
        INatsPublisher natsPublisher,
        ISessionRecordingService sessionRecordingService,
        string sessionId,
        SIPTransport transport
    ) : base(
        callScope,
        logger,
        sessionOptions.Value.AccessUrl,
        sessionOptions.Value.PredictiveTimeouts.SessionTimeout,
        natsSubjects,
        generalOptions,
        leadConnectionFactory,
        natsPublisher,
        sessionRecordingService,
        sessionId,
        transport
    )
    {
        _timeouts = sessionOptions.Value.PredictiveTimeouts;
    }

    public override async Task Start(InitCallData initCallData)
    {
        _maxCallDuration = initCallData.MaxCallDuration;
        _maxRingingTime = initCallData.RingingTimeout;
        await base.Start(initCallData);
        await InviteLead();
    }

    public override void OnAgentConnectionLost()
    {
        if (_agentDropped) return;
        base.OnAgentConnectionLost();
        _agentTimeout = new Timer(AgentReconnectTimeout, null, _timeouts.AgentReconnectTimeout, Timeout.Infinite);
    }

    public override async Task Close(CloseCommand cmd)
    {
        _ringingTimeout?.Dispose();
        _callDurationTimeout?.Dispose();
        _sessionTimeout?.Dispose();
        _agentTimeout?.Dispose();
        await base.Close(cmd);
    }

    protected override async Task AgentAnswer()
    {
        _sessionTimeout?.Dispose();
        _agentTimeout?.Dispose();
        if (_maxCallDuration.HasValue)
            _callDurationTimeout = new Timer(MaxCallDurationTimeout, null, _maxCallDuration.Value * 1000, Timeout.Infinite);
        await base.AgentAnswer();
    }

    protected override bool DropAgent(string? reason = null)
    {
        if (base.DropAgent(reason))
        {
            _agentTimeout?.Dispose();
            return true;
        }
        return false;
    }

    protected override async Task LeadAnswer()
    {
        _ringingTimeout?.Dispose();
        await base.LeadAnswer();
        await InviteAgent();
        _agentTimeout = new Timer(AgentTimeout, null, _timeouts.AgentTimeout, Timeout.Infinite);
    }

    protected override void LeadRinging()
    {
        if (_maxRingingTime.HasValue)
            _ringingTimeout = new Timer(LeadTimeout, null, _maxRingingTime.Value * 1000, Timeout.Infinite);
        base.LeadRinging();
    }

    protected async void LeadTimeout(object? _)
    {
        var cmd = new CloseCommand(CallFinishReasons.LeadNotAnswer);
        await Close(cmd);
    }

    protected async void AgentTimeout(object? state)
    {
        _agentTimeout?.Dispose();
        _agentTimeout = new Timer(AgentTimeoutWithClose, null, _timeouts.AgentTimeout, Timeout.Infinite);
        _logger.LogDebug("AgentNotAnsweredMessage AgentId={agentId}, LeadId={leadId}, IsFixedAssigned={isFixed}, LeadPhone={leadPhone}, Session={session}",
                        CallData.AgentId, CallData.LeadId, CallData.IsFixedAssigned, CallData.LeadPhone, Id);

        await _natsPublisher.PublishAsync(_natsSubjects.AgentNotAnswered,
            new AgentNotAnsweredMessage(
                CallData.ClientId,
                _generalOptions.InstanceId,
                Id,
                CallData.CallType,
                CallData.QueueId,
                CallData.AgentId,
                CallData.LeadId,
                CallData.IsFixedAssigned,
                CallData.LeadPhone,
                CallData.SipProvider.Id
            ));
    }

    protected async void AgentTimeoutWithClose(object? state)
    {
        _logger.LogDebug("AgentAnswer AgentId={agentId}, LeadId={leadId}, IsFixedAssigned={isFixed}, LeadPhone={leadPhone}, Session={session}",
                        CallData.AgentId, CallData.LeadId, CallData.IsFixedAssigned, CallData.LeadPhone, Id);
        var cmd = new CloseCommand(CallFinishReasons.AgentTimeout);
        await Close(cmd);
    }

    protected async void AgentReconnectTimeout(object? state)
    {
        var cmd = new CloseCommand(CallFinishReasons.AgentReconnectTimeout);
        await Close(cmd);
    }

    protected async void MaxCallDurationTimeout(object? state)
    {
        var cmd = new CloseCommand(CallFinishReasons.ExceededMaxCallDuration);
        await Close(cmd);
    }
}
