using KL.SIP.Bridge.Application.Configurations;
using KL.SIP.Bridge.Application.Enums;
using Microsoft.Extensions.Options;
using Plat4me.Core.Nats;
using SIPSorcery.SIP;

namespace KL.SIP.Bridge.Application.Session
{
    public class CallSessionBuilder
    {
        private readonly ILogger<ICallSession> _logger;
        private readonly IOptions<CallSessionOptions> _sessionOptions;
        private readonly IOptionsMonitor<SIPOptions> _sipOptions;
        private readonly IOptions<NatsSubjects> _natsSubjects;
        private readonly IOptions<GeneralOptions> _generalOptions;
        private readonly IOptions<FakeCallOptions> _fakeCallOptions;
        private readonly INatsPublisher _natPublisher;
        private readonly ILeadConnectionFactory _leadConnectionFactory;

        private IServiceScope _callScope;

        public CallSessionBuilder(
            ILogger<ICallSession> logger,
            IOptions<CallSessionOptions> sessionOptions,
            IOptionsMonitor<SIPOptions> sipOptions,
            IOptions<NatsSubjects> natsSubjects,
            IOptions<GeneralOptions> generalOptions,
            IOptions<FakeCallOptions> fakeCallOptions,
            INatsPublisher natPublisher,
            IServiceProvider serviceProvider
            )
        {
            _sessionOptions = sessionOptions;
            _sipOptions = sipOptions;
            _natsSubjects = natsSubjects;
            _generalOptions = generalOptions;
            _fakeCallOptions = fakeCallOptions;
            _natPublisher = natPublisher;
            _logger = logger;
            _callScope = serviceProvider.CreateScope();
            _leadConnectionFactory = new LeadConnectionFactory(_fakeCallOptions, _sipOptions, _logger);
        }

        public ICallSession Build(string sessionId, InitCallData callData, SIPTransport transport)
        {
            var sessionRecording = _callScope.ServiceProvider.GetRequiredService<ISessionRecordingService>();
            switch (callData.CallType)
            {
                case CallType.Predictive:
                    return new PredictiveCallSession(
                        _callScope,
                        _logger,
                        _sessionOptions,
                        _natsSubjects,
                        _generalOptions,
                        _leadConnectionFactory,
                        _natPublisher,
                        sessionRecording,
                        sessionId,
                        transport
                    );
                case CallType.Manual:
                    return new ManualCallSession(
                        _callScope,
                        _logger,
                        _sessionOptions,
                        _natsSubjects,
                        _generalOptions,
                        _leadConnectionFactory,
                        _natPublisher,
                        sessionRecording,
                        sessionId,
                        transport
                    );
            }
            throw new InvalidOperationException("Can not create session of unknown type");
        }
    }
}
