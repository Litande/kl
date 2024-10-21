using Plat4Me.DialRuleEngine.Application.Handlers.Contracts;
using Plat4Me.DialRuleEngine.Application.Models.Messages;

namespace Plat4Me.DialRuleEngine.Application.Handlers;

public class LeadFeedbackCallFailedHandler : ILeadFeedbackCallFailedHandler
{
    private readonly ILeadFeedbackFilledHandler _feedbackFilledHandler;

    public LeadFeedbackCallFailedHandler(ILeadFeedbackFilledHandler feedbackFilledHandler)
    {
        _feedbackFilledHandler = feedbackFilledHandler;
    }

    public async Task Process(LeadFeedbackFilledMessage message) =>
        await _feedbackFilledHandler.Process(message);
}
