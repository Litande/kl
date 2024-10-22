using KL.SIP.Bridge.Application.Configurations;
using KL.SIP.Bridge.Application.Enums;
using Microsoft.Extensions.Options;
using Plat4me.Core.Nats;
using SIPSorcery.SIP;

namespace KL.SIP.Bridge.Application.Session;

public class ManualCallSession : CallSessionBase
{
    protected Timer? _agentTimeout;
    protected Timer? _ringingTimeout;
    protected long? _maxRingingTime;
    protected Timer? _callDurationTimeout;
    protected long? _maxCallDuration;
    protected ManualTimeouts _timeouts;
    public ManualCallSession
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
        sessionOptions.Value.ManualTimeouts.SessionTimeout,
        natsSubjects,
        generalOptions,
        leadConnectionFactory,
        natsPublisher,
        sessionRecordingService,
        sessionId,
        transport
    )
    {
        _timeouts = sessionOptions.Value.ManualTimeouts;
    }

    public override async Task Start(InitCallData initCallData)
    {
        _maxCallDuration = initCallData.MaxCallDuration;
        _maxRingingTime = initCallData.RingingTimeout;
        await base.Start(initCallData);
        await InviteAgent();
    }

    public override void OnAgentConnectionLost()
    {
        if (_agentDropped) return;
        base.OnAgentConnectionLost();
        _agentTimeout = new Timer(AgentReconnectTimeout, null, _timeouts.AgentReconnectTimeout, Timeout.Infinite);
    }

    public override async Task Close(CloseCommand cmd)
    {
        _callDurationTimeout?.Dispose();
        _sessionTimeout?.Dispose();
        _agentTimeout?.Dispose();
        _ringingTimeout?.Dispose();
        await base.Close(cmd);
    }

    protected override async Task AgentAnswer()
    {
        _agentTimeout?.Dispose();
        await base.AgentAnswer();
        if (_leadConnection is null)
        {
            await InviteLead();
        }
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

    protected async void LeadTimeout(object? _)
    {
        var cmd = new CloseCommand(CallFinishReasons.LeadNotAnswer);
        await Close(cmd);
    }

    protected override void LeadRinging()
    {
        if (_maxRingingTime.HasValue)
            _ringingTimeout = new Timer(LeadTimeout, null, _maxRingingTime.Value * 1000, Timeout.Infinite);
        base.LeadRinging();
    }

    protected override async Task LeadAnswer()
    {
        _sessionTimeout?.Dispose();
        _ringingTimeout?.Dispose();
        if (_maxCallDuration.HasValue)
            _callDurationTimeout = new Timer(MaxCallDurationTimeout, null, _maxCallDuration.Value * 1000, Timeout.Infinite);
        await base.LeadAnswer();
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
