using  Plat4Me.DialSipBridge.Application.Services;

namespace Plat4Me.DialSipBridge.Application.Session;

public interface ISessionRecordingService
{
    void Init(string sessionId, string leadPhone);
    Task<IAudioStreamRecorder?> CreateLeadRecorder();
    Task<IAudioStreamRecorder?> CreateManagerRecorder();
    Task<IAudioStreamRecorder?> CreateAgentRecorder();
    void StopRecording(IAudioStreamRecorder? recorder);
    Task WaitAllRecordings();
}
