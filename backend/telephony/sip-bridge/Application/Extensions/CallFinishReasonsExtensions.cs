using KL.SIP.Bridge.Application.Enums;

namespace KL.SIP.Bridge.Application.Extensions;

internal static class CallFinishReasonsExtensions
{
    public static bool IsSuccessFinish(this CallFinishReasons reason)
    {
        return reason is CallFinishReasons.CallFinishedByAgent or CallFinishReasons.CallFinishedByLead or CallFinishReasons.CallFinishedByManager;
    }
}
