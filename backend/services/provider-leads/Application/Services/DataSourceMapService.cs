using KL.Provider.Leads.Application.Models;
using KL.Provider.Leads.Application.Services.Interfaces;
using KL.Provider.Leads.Persistent.Entities;
using KL.Provider.Leads.Persistent.Repositories.Interfaces;

namespace KL.Provider.Leads.Application.Services;

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