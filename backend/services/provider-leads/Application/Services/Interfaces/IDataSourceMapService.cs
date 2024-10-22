using KL.Provider.Leads.Application.Models;
using KL.Provider.Leads.Persistent.Entities;

namespace KL.Provider.Leads.Application.Services.Interfaces;

public interface IDataSourceMapService
{
    IEnumerable<UserDataSourceMap> GetUserDataSources();
    IEnumerable<LeadDataSourceMap> GetLeadDataSources();
    IEnumerable<StatusDataSourceMap> GetStatusDataSources();
    IEnumerable<TimeZoneProjection> GetTimeZones();
}