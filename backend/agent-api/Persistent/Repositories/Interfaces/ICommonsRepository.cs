using Plat4Me.DialAgentApi.Application.Models.Responses.Common;

namespace Plat4Me.DialAgentApi.Persistent.Repositories.Interfaces;

public interface ICommonsRepository
{
    IReadOnlyCollection<LabelValue> Countries { get; }
    IReadOnlyCollection<LabelValue> LeadStatuses { get; }
}
