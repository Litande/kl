using KL.SIP.Bridge.Application.Enums;
using SIPSorcery.Net;

namespace KL.SIP.Bridge.Application.Connections;

public interface IConnection : IDisposable
{
    event Action<CallFinishReasons>? OnCallFailed;
    event Action? OnCallAnswered;
    event Action? OnCallHangup;
    event Action? OnCallRinging;
    event Action<IConnection, SDPMediaTypesEnum, RTPPacket>? OnData;
    int? LastErrorCode { get; }
    void SendToAudioStream(int audioStreamIdx, RTPPacket packet);
    Task Start();
    void Hangup();
}
