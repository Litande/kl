using Plat4Me.DialSipBridge.Application.Connections;
using SIPSorcery.SIP;

namespace Plat4Me.DialSipBridge.Application.Session;

public interface ILeadConnectionFactory
{
    IConnection CreateLeadConnection(InitCallData callData, SIPTransport transport);
}
