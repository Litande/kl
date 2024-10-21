using Plat4Me.Dial.Statistic.Api.Application.Models.Entities;
using Plat4Me.Dial.Statistic.Api.Application.Models.Messages;

namespace Plat4Me.Dial.Statistic.Api.Application.Common.Extensions;

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