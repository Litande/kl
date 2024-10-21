using SIPSorcery.Net;

namespace Plat4Me.DialSipBridge.Application.Services;

public interface IAudioStreamRecorder : IDisposable
{
    public const int PCMASamplingRate = 8000;
    public const int PCMAPacketsPerSecond = 50;
    public const int PCMAPacketSamplesCount = PCMASamplingRate / PCMAPacketsPerSecond;
    public static readonly byte[] SilenceData = Enumerable.Repeat((byte)0x55, PCMAPacketSamplesCount).ToArray();
 
    string? RecordName { get; }

    void EnqueuePacket(RTPPacket packet);
}
