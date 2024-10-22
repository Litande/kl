using KL.Provider.Leads.Application.Models.Messages;

namespace KL.Provider.Leads.Application.Handlers.Interfaces;

public interface ILeadFeedbackProcessedHandler : ISubHandler<LeadFeedbackProcessedMessage>
{

}