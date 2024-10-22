using KL.Statistics.Application.Models.Entities;
using KL.Statistics.Application.Models.Messages;

namespace KL.Statistics.Application.Common.Extensions;

public static class CallDetailRecordsExtensions
{
    public static List<CdrUpdatedMessage> ToCdrChangedMessage(this IReadOnlyCollection<CallDetailRecord>? callDetailRecords)
        => callDetailRecords?.Select(i => new CdrUpdatedMessage(
               i.SessionId,
               i.ClientId,
               i.LeadCountry,
               i.UserId,
               i.OriginatedAt,
               i.CallHangupAt,
               i.LeadAnswerAt,
               i.UserAnswerAt,
               i.LeadStatusAfter)).ToList() ??
           new List<CdrUpdatedMessage>();
}