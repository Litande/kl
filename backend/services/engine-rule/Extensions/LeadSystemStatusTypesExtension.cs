using KL.Engine.Rule.Enums;

namespace KL.Engine.Rule.Extensions;

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
