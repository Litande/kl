using KL.Manager.API.Application.Models.Responses.Common;

namespace KL.Manager.API.Persistent.Repositories.Interfaces;

public interface ICommonsRepository
{
    IReadOnlyCollection<LabelValue> Countries { get; }
    IReadOnlyCollection<LabelValue> LeadStatuses { get; }
}
