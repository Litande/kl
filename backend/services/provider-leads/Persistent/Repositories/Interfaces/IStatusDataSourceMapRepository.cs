using Plat4Me.DialLeadProvider.Application.Enums;
using Plat4Me.DialLeadProvider.Persistent.Entities;

namespace Plat4Me.DialLeadProvider.Persistent.Repositories.Interfaces;

public interface IStatusDataSourceMapRepository
{
    IEnumerable<StatusDataSourceMap> GetStatusDataSourceMap();
    Task<StatusDataSourceMap?> GetByStatus(LeadStatusTypes status, CancellationToken ct = default);
}