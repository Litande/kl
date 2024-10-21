using Plat4Me.DialLeadCaller.Application.Enums;

namespace Plat4Me.DialLeadCaller.Application.Extensions;

public class CallStatusTypesExtension
{
    public static IEnumerable<CallFinishReasons> Success => new[]
    {
        CallFinishReasons.CallFinishedByAgent,
        CallFinishReasons.CallFinishedByLead,
        CallFinishReasons.CallFinishedByManager,
        // TODO add other success statuses
    };
}
