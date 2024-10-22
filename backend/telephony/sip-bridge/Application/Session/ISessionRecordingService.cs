using KL.SIP.Bridge.Application.Services;

namespace KL.SIP.Bridge.Application.Session;

public interface ISessionRecordingService
{
    void Init(string sessionId, string leadPhone);
    Task<IAudioStreamRecorder?> CreateLeadRecorder();
    Task<IAudioStreamRecorder?> CreateManagerRecorder();
    Task<IAudioStreamRecorder?> CreateAgentRecorder();
    void StopRecording(IAudioStreamRecorder? recorder);
    Task WaitAllRecordings();
}
