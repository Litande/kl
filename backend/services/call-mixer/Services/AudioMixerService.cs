using System.Text;
using FFMpegCore;
using FFMpegCore.Pipes;

namespace KL.Call.Mixer.Services;


public class AudioMixerService : IAudioMixerService
{
    public string ContentType => "application/ogg";
    protected const string FileExtension = ".opus";
    protected const string OutputFormatParams = "-c:a libopus -b:a 24K -f ogg";
    private const string RecordDateTimeFormat = "yyyyMMdd'T'HHmmss.fff";


    protected readonly ILogger<AudioMixerService> _logger;

    public AudioMixerService(
        ILogger<AudioMixerService> logger
    )
    {
        _logger = logger;
    }

    public async Task<string?> MixCallRecords(
        string sessionId,
        KeyValuePair<string, Stream> leadRecord,
        IEnumerable<KeyValuePair<string, Stream>>? userRecords,
        IEnumerable<KeyValuePair<string, Stream>>? managerRecords,
        Stream output,
        CancellationToken ct = default)
    {
        var leadRecordDescriptor = CreateCallRecordDescriptor(leadRecord.Key, leadRecord.Value);
        if (string.IsNullOrWhiteSpace(sessionId))
            throw new ArgumentNullException(nameof(sessionId));
        if (string.IsNullOrWhiteSpace(leadRecordDescriptor.LeadPhone))
            throw new ArgumentNullException(nameof(leadRecordDescriptor.LeadPhone));

        List<CallRecordDescriptor> mixingRecords = new();

        mixingRecords.Add(leadRecordDescriptor);
        if (userRecords is not null)
            mixingRecords.AddRange(userRecords.Select(x =>
               CreateCallRecordDescriptor(x.Key, x.Value)));
        if (managerRecords is not null)
            mixingRecords.AddRange(managerRecords.Select(x =>
               CreateCallRecordDescriptor(x.Key, x.Value)));

        mixingRecords = mixingRecords.OrderBy(x => x.RecordStartAt).ToList();
        var baseRecord = mixingRecords.First();
        mixingRecords.RemoveAt(0);

        if (mixingRecords.Any())
            await MixRecords(
                sessionId,
                leadRecordDescriptor.LeadPhone,
                baseRecord,
                mixingRecords,
                output,
                ct);
        else
        {
            _logger.LogDebug("Nothing to mix");
            return null;
        }

        return $"{sessionId}/{baseRecord.RecordStartAt.ToString(RecordDateTimeFormat)}_{leadRecordDescriptor.LeadPhone}{FileExtension}";
    }

    private record CallRecordDescriptor(
        string LeadPhone,
        DateTimeOffset RecordStartAt,
        string RecordType,
        string Filename,
        Stream Stream
    );

    private CallRecordDescriptor CreateCallRecordDescriptor(string recordName, Stream stream)
    {
        var recordNameFields = Path.GetFileNameWithoutExtension(recordName).Split('_');
        if (recordNameFields.Count() < 3)
            throw new ArgumentException("invalid file name");
        var timestamp = DateTimeOffset.ParseExact(recordNameFields[0], RecordDateTimeFormat, null);
        return new CallRecordDescriptor(recordNameFields[1], timestamp, recordNameFields[2], recordName, stream);
    }

    private async Task MixRecords(
        string sessionId,
        string leadPhone,
        CallRecordDescriptor baseRecord,
        List<CallRecordDescriptor> mixingRecords,
        Stream output,
        CancellationToken ct = default
    )
    {
        FFMpegArguments args = null!;
        try
        {
            args = FFMpegArguments.FromPipeInput(new StreamPipeSource(baseRecord.Stream), (opts) =>
           {
               opts.WithCustomArgument(GetInputFileArgsByExtension(baseRecord.Filename));
           }
            );

            var outputSink = new StreamPipeSink(output);
            var outputParams = new StringBuilder("-filter_complex \"");
            int inputIdx = 1;// 0 - base stream
            List<string> inputStreams = new();
            foreach (var callRecordDescriptor in mixingRecords)
            {
                args.AddPipeInput(new StreamPipeSource(callRecordDescriptor.Stream), (opts) =>
                {
                    opts.WithCustomArgument(GetInputFileArgsByExtension(callRecordDescriptor.Filename));
                });
                var delay = callRecordDescriptor.RecordStartAt - baseRecord.RecordStartAt;
                var inputStream = $"[a{inputIdx}]";
                outputParams.Append($"[{inputIdx}]adelay={(int)delay.TotalMilliseconds}{inputStream};"); //[1]adelay=msec[a1]; 
                inputStreams.Add(inputStream);
                ++inputIdx;
            }
            outputParams.Append("[0:a]").AppendJoin(null, inputStreams);//base stream[0] and list of "delayed" streams 
            outputParams.Append($"amix=inputs={inputIdx}:duration=longest:dropout_transition=0,dynaudnorm[a]\" -map \"[a]\" "); //
            outputParams.Append(OutputFormatParams);
            await args
                .OutputToPipe(outputSink, opts =>
                {
                    var resultParams = outputParams.ToString();
                    opts.WithCustomArgument(resultParams);
                    _logger.LogDebug(resultParams);
                })
                .ProcessAsynchronously();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "MixRecords failed\n{arguments}", args?.Text);
        }
    }

    private string GetInputFileArgsByExtension(string filename)
    {
        if (filename.EndsWith(".opus"))
        {
            return "-f ogg"; ///-ar 8000 -ac 1
        }
        if (filename.EndsWith(".pcma"))
        {
            return "-f alaw -ar 8000 -ac 1";
        }
        throw new ArgumentException("Unknown file extension");
    }
}
