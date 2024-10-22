using KL.SIP.Bridge.Application.Configurations;
using KL.SIP.Bridge.Application.Connections;
using Microsoft.Extensions.Options;
using SIPSorcery.SIP;

namespace KL.SIP.Bridge.Application.Session
{
    public class LeadConnectionFactory : ILeadConnectionFactory
    {
        private readonly ILogger<ICallSession> _logger;
        private readonly IOptionsMonitor<SIPOptions> _sipOptions;
        private readonly FakeCallOptions _fakeCallOptions;

        public LeadConnectionFactory(
            IOptions<FakeCallOptions> fakeCallOptions,
            IOptionsMonitor<SIPOptions> sipOptions,
            ILogger<ICallSession> logger
        )
        {
            _fakeCallOptions = fakeCallOptions.Value;
            _sipOptions = sipOptions;
            _logger = logger;
        }

        public IConnection CreateLeadConnection(InitCallData callData, SIPTransport transport)
        {
            if (callData.IsTest)
                return new FakeSIPConnection(callData.LeadPhone, _fakeCallOptions, _logger);
            else
                return new LeadSIPConnection(callData.LeadPhone, callData.SipProvider, _sipOptions.CurrentValue, transport, _logger);
        }
    }
}
