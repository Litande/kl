using KL.SIP.Bridge.Application.Models;

namespace KL.SIP.Bridge.Application.Services;

public interface IUploaderService
{
    void UploadRecords(UploadRecordsRequest req);
}
