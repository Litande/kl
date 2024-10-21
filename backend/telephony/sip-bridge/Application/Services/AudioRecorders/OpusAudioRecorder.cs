using Concentus.Enums;
using Concentus.Oggfile;
using Concentus.Structs;
using SIPSorcery.Media;
using Microsoft.Extensions.Logging;

namespace Plat4Me.DialSipBridge.Application.Services.AudioRecorders;

public class OpusAudioRecorder : AudioRecorder
{
    protected override string FileExtension => OggFileExtension;
    private readonly OpusEncoder _encoder;
    private const string OggFileExtension = ".opus";
    private OpusOggWriteStream? _oggOutStream;
    private FileStream? _outputFile = null!;

    public OpusAudioRecorder(
       ILogger logger,
       string sessionId,
       string leadPhone,
       string endpoint,
       string outputDirectory
    ) : base(logger, sessionId, leadPhone, endpoint, outputDirectory)
    {
        _encoder = OpusEncoder.Create(IAudioStreamRecorder.PCMASamplingRate, 1, OpusApplication.OPUS_APPLICATION_VOIP);
        _encoder.Bitrate = 24 * 1024;//10k narrowband - 24k fullband
        _encoder.SignalType = OpusSignal.OPUS_SIGNAL_VOICE;
        _encoder.Complexity = 5;//0 -lowest cpu/quality , 10 - highest cpu/quality
    }

    protected override void InitOutputFile()
    {
        _outputFile = File.Open(LocalFileName, FileMode.Create);///??? FileMode.CreateNew
        if (_outputFile is not null)
            _oggOutStream = new OpusOggWriteStream(_encoder, _outputFile);
    }

    protected override void CloseOutputFile()
    {

        if (_outputFile is not null)
        {
            _oggOutStream?.Finish();
            _oggOutStream = null;
            _outputFile.Close();
            _outputFile = null;
        }

    }

    protected override Task SaveAudioData(byte[] samples, int length)
    {
        _oggOutStream?.WriteSamples(
            samples.Select(x => ALawDecoder.ALawToLinearSample(x)).ToArray(),
            0,
            length
        );
        return Task.CompletedTask;
    }
}
