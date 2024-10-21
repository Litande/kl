using Plat4Me.DialSipBridge.Application.Enums;

namespace Plat4Me.DialSipBridge.Application.Extensions;

internal static class CallFinishReasonsExtensions
{
    public static bool IsSuccessFinish(this CallFinishReasons reason)
    {
        return reason is CallFinishReasons.CallFinishedByAgent or CallFinishReasons.CallFinishedByLead or CallFinishReasons.CallFinishedByManager;
    }
}
