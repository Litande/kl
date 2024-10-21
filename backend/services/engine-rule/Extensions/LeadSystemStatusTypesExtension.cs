using Plat4Me.DialRuleEngine.Application.Enums;

namespace Plat4Me.DialRuleEngine.Application.Extensions;

public class LeadSystemStatusTypesExtension
{
    public static IReadOnlyCollection<LeadSystemStatusTypes> Unavailable => new[]
    {
        LeadSystemStatusTypes.WaitingFeedback,
        LeadSystemStatusTypes.Processing,
        LeadSystemStatusTypes.InTheCall,
        LeadSystemStatusTypes.PostProcessing,
        LeadSystemStatusTypes.Imported,
        LeadSystemStatusTypes.Dialing,
    };
}
