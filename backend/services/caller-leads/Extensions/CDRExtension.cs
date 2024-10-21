using System.Diagnostics.CodeAnalysis;
using Plat4Me.DialLeadCaller.Application.Models.Entities;
using Plat4Me.DialLeadCaller.Application.Models.Messages;

namespace Plat4Me.DialLeadCaller.Application.Extensions;

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