using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;

namespace KL.Storage.GoogleCouldStorage;

public class GoogleCloudStorageService(IOptions<GoogleStorageOptions> options) : IStorageService
{
    private readonly Lazy<StorageClient> _client = new(StorageClient.Create);
    private readonly GoogleStorageOptions _options = options.Value;

    public async Task Upload(string fileName, string contentType, Stream source, bool closeStream = true, CancellationToken ct = default)
    {
        var storage = _client.Value;

        await storage.UploadObjectAsync(
            _options.BucketName,
            fileName,
            contentType,
            source,
            cancellationToken: ct);

        if (closeStream)
            source.Close();
    }

    public async Task<Stream> Download(string fileName, CancellationToken ct)
    {
        var storage = _client.Value;

        var fileStream = new MemoryStream();
        await storage.DownloadObjectAsync(
            _options.BucketName,
            fileName,
            fileStream,
            cancellationToken: ct);

        fileStream.Position = 0;

        return fileStream;
    }

    public async Task Delete(string fileName, CancellationToken ct = default)
    {
        var storage = _client.Value;

        await storage.DeleteObjectAsync(
            _options.BucketName,
            fileName,
            cancellationToken: ct);
    }

    public void Dispose()
    {
        if (_client.IsValueCreated)
        {
            var storage = _client.Value;

            storage.Dispose();
        }
    }
}
