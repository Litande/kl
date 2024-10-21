using Plat4Me.DialLeadProvider.Application.Models;
using Plat4Me.DialLeadProvider.Application.Services.Interfaces;
using Plat4Me.DialLeadProvider.Persistent.Entities;
using Plat4Me.DialLeadProvider.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialLeadProvider.Application.Services;

public class DataSourceMapService : IDataSourceMapService
{
    private readonly IStatusDataSourceMapRepository _dataSourceMapRepository;
    private readonly ILeadDataSourceMapRepository _leadDataSourceMapRepository;
    private readonly IUserDataSourceMapRepository _userDataSourceMapRepository;
    private readonly ITimeZoneRepository _timeZoneRepository;

    public DataSourceMapService(
        IStatusDataSourceMapRepository dataSourceMapRepository,
        ILeadDataSourceMapRepository leadDataSourceMapRepository,
        IUserDataSourceMapRepository userDataSourceMapRepository,
        ITimeZoneRepository timeZoneRepository)
    {
        _dataSourceMapRepository = dataSourceMapRepository;
        _leadDataSourceMapRepository = leadDataSourceMapRepository;
        _userDataSourceMapRepository = userDataSourceMapRepository;
        _timeZoneRepository = timeZoneRepository;
    }

    public IEnumerable<UserDataSourceMap> GetUserDataSources()
    {
        var userDataSourceMap = _userDataSourceMapRepository
            .GetUserDataSourceMap();
        return userDataSourceMap;
    }

    public IEnumerable<LeadDataSourceMap> GetLeadDataSources()
    {
        var leadDataSourceMap = _leadDataSourceMapRepository
            .GetLeadDataSourceMap();
        return leadDataSourceMap;
    }

    public IEnumerable<StatusDataSourceMap> GetStatusDataSources()
    {
        var statusDataSources = _dataSourceMapRepository
            .GetStatusDataSourceMap();
        return statusDataSources;
    }

    public IEnumerable<TimeZoneProjection> GetTimeZones()
    {
        var timeZones = _timeZoneRepository
            .GetTimeZones();
        return timeZones;
    }
}