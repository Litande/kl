using Plat4Me.DialLeadProvider.Application.Models;
using Plat4Me.DialLeadProvider.Persistent.Entities;

namespace Plat4Me.DialLeadProvider.Application.Services.Interfaces;

public interface IDataSourceMapService
{
    IEnumerable<UserDataSourceMap> GetUserDataSources();
    IEnumerable<LeadDataSourceMap> GetLeadDataSources();
    IEnumerable<StatusDataSourceMap> GetStatusDataSources();
    IEnumerable<TimeZoneProjection> GetTimeZones();
}