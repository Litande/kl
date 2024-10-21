using Plat4Me.DialClientApi.Application.Models.Responses.Common;

namespace Plat4Me.DialClientApi.Persistent.Repositories.Interfaces;

public interface ICommonsRepository
{
    IReadOnlyCollection<LabelValue> Countries { get; }
    IReadOnlyCollection<LabelValue> LeadStatuses { get; }
}
