using KL.Engine.Rule.Handlers.Contracts;
using KL.Engine.Rule.Models.Messages;

namespace KL.Engine.Rule.Handlers;

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
