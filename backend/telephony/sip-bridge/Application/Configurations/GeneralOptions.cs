using Plat4Me.DialSipBridge.Application.Enums;

namespace Plat4Me.DialSipBridge.Application.Configurations;

public class GeneralOptions
{
    public string InstanceId { get; set; } = Guid.NewGuid().ToString();
    public bool RecordingEnabled { get; set; }
    public bool ManagerRecordingEnabled { get; set; }
    public string RecordingTemporaryDir { get; set; } = null!;
    public string RecordingStorePrefix { get; set; } = null!;
    public AudioRecordFormat RecordingFormat { get; set; }
}
