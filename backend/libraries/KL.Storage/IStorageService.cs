namespace KL.Storage;

public interface IStorageService : IDisposable
{
    Task Upload(string fileName, string contentType, Stream source, bool closeStream, CancellationToken ct);
    Task<Stream> Download(string fileName, CancellationToken ct);
    Task Delete(string fileName, CancellationToken ct = default);
}
