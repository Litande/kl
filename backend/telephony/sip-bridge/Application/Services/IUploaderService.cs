using Plat4Me.DialSipBridge.Application.Models;

namespace Plat4Me.DialSipBridge.Application.Services;

public interface IUploaderService
{
    void UploadRecords(UploadRecordsRequest req);
}
