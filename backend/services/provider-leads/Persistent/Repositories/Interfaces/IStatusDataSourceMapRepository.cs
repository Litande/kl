using KL.Provider.Leads.Application.Enums;
using KL.Provider.Leads.Persistent.Entities;

namespace KL.Provider.Leads.Persistent.Repositories.Interfaces;

public interface IStatusDataSourceMapRepository
{
    IEnumerable<StatusDataSourceMap> GetStatusDataSourceMap();
    Task<StatusDataSourceMap?> GetByStatus(LeadStatusTypes status, CancellationToken ct = default);
}