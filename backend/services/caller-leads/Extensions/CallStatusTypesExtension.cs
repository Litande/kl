using KL.Caller.Leads.Enums;

namespace KL.Caller.Leads.Extensions;

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
