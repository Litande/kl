using System.Net;
using SIPSorcery.Net;

namespace KL.SIP.Bridge.Application.Connections;

public class ExternalRTPSession : RTPSession
{
    public IPAddress ExternalIP { get; protected set; }
    public ExternalRTPSession(IPAddress externalIp, bool isMediaMultiplexed,
        bool isRtcpMultiplexed, bool isSecure,
        IPAddress? bindAddress = null, int bindPort = 0, SIPSorcery.Sys.PortRange? portRange = null)
        : base(isMediaMultiplexed, isRtcpMultiplexed, isSecure, bindAddress, bindPort, portRange)
    {
        ExternalIP = externalIp;
    }

    public override SDP CreateOffer(IPAddress connectionAddress)
    {
        var res = base.CreateOffer(ExternalIP);
        return res;
    }
}
