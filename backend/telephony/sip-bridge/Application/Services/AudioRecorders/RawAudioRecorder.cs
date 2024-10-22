namespace KL.SIP.Bridge.Application.Services.AudioRecorders;

public class RawAudioRecorder : AudioRecorder
{
    protected override string FileExtension => RawFileExtension;

    private const string RawFileExtension = ".pcma";

    protected FileStream? _outputFile = null!;

    public RawAudioRecorder(
        ILogger logger,
        string sessionId,
        string leadPhone,
        string endpoint,
        string outputDirectory
    ) : base(logger, sessionId, leadPhone, endpoint, outputDirectory)
    {
    }

    protected override void InitOutputFile()
    {
        _outputFile = File.Open(LocalFileName, FileMode.Create);///??? FileMode.CreateNew
    }

    protected override void CloseOutputFile()
    {
        if (_outputFile is not null)
        {
            _outputFile.Close();
            _outputFile = null;
        }
    }

    protected override async Task SaveAudioData(byte[] samples, int length)
    {
        if (_outputFile is not null)
        {
            await _outputFile.WriteAsync(samples, 0, length);
            /// await _outputFile.FlushAsync();
        }
    }

}
