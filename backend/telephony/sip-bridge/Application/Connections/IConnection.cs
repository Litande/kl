using Plat4Me.DialSipBridge.Application.Enums;
using SIPSorcery.Net;

namespace Plat4Me.DialSipBridge.Application.Connections;

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
