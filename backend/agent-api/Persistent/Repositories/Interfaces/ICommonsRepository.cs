using KL.Agent.API.Application.Models.Responses.Common;

namespace KL.Agent.API.Persistent.Repositories.Interfaces;

public interface ICommonsRepository
{
    IReadOnlyCollection<LabelValue> Countries { get; }
    IReadOnlyCollection<LabelValue> LeadStatuses { get; }
}
