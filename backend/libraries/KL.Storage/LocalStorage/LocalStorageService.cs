using Microsoft.Extensions.Options;

namespace KL.Storage.LocalStorage;

public class LocalStorageService(IOptions<LocalStorageOptions> options) : IStorageService
{
    private readonly LocalStorageOptions _options = options.Value;

    public async Task Upload(string fileName, string contentType, Stream source, bool closeStream = true, CancellationToken ct = default)
    {
        var fullPath = Path.Combine(_options.Path, fileName);

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

        await using (var file = File.Create(fullPath))
        {
            await source.CopyToAsync(file, ct);
        }

        if (closeStream)
        {
            source.Close();
        }
    }

    public async Task<Stream> Download(string fileName, CancellationToken ct)
    {
        var fullPath = Path.Combine(_options.Path, fileName);

        if (!File.Exists(fullPath))
        {
            throw new Exception($"The {fileName} file is not found");
        }

        return File.OpenRead(fullPath);
    }

    public async Task Delete(string fileName, CancellationToken ct = default)
    {
        var fullPath = Path.Combine(_options.Path, fileName);

        if (!File.Exists(fullPath))
        {
            throw new Exception($"The {fileName} file is not found");
        }

        File.Delete(fullPath);
    }

    public void Dispose()
    {
    }
}
