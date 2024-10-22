using KL.Statistics.Application.Models.Messages;

namespace KL.Statistics.Application.Handlers.LeadStatistics;

public interface ILeadStatisticsChangeHandler : ISubHandler<LeadsStatisticUpdateMessage>
{
}
