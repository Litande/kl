using KL.Statistics.Application.Models.Messages;

namespace KL.Statistics.Application.Models.StatisticCache;

public class CdrStatisticCache
{
    public CdrStatisticCache(
        List<CdrUpdatedMessage> callDetailRecords,
        DateTime insertedDate)
    {
        CallDetailRecords = callDetailRecords;
        InsertedDate = insertedDate;
    }

    public List<CdrUpdatedMessage> CallDetailRecords { get; set; } = new();
    public DateTime InsertedDate { get; set; }
}