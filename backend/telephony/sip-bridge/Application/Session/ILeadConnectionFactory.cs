using KL.SIP.Bridge.Application.Connections;
using SIPSorcery.SIP;

namespace KL.SIP.Bridge.Application.Session;

public interface ILeadConnectionFactory
{
    IConnection CreateLeadConnection(InitCallData callData, SIPTransport transport);
}
