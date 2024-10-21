using Plat4Me.DialLeadProvider.Persistent.Entities;

namespace Plat4Me.DialLeadProvider.Persistent.Repositories.Interfaces;

public interface ILeadDataSourceMapRepository
{
    IEnumerable<LeadDataSourceMap> GetLeadDataSourceMap();
}