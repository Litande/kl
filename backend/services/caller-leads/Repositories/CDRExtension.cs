using System.Diagnostics.CodeAnalysis;
using KL.Caller.Leads.Models.Entities;
using KL.Caller.Leads.Models.Messages;

namespace KL.Caller.Leads.Extensions;

public static class CDRExtension
{
    [return: NotNullIfNotNull("cdr")]
    public static CdrUpdatedMessage? MapToMessage(this CallDetailRecord? cdr)
    {
        if (cdr is null) return null;
        return new CdrUpdatedMessage(
            cdr.SessionId,
            cdr.ClientId,
            cdr.LeadCountry,
            cdr.LastUserId,
            cdr.OriginatedAt,
            cdr.CallHangupAt,
            cdr.LeadAnswerAt,
            cdr.UserAnswerAt,
            cdr.LeadStatusAfter);
    }
}