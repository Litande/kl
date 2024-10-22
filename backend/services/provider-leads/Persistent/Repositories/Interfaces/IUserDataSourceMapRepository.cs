using KL.Provider.Leads.Persistent.Entities;

namespace KL.Provider.Leads.Persistent.Repositories.Interfaces;

public interface IUserDataSourceMapRepository
{
    IEnumerable<UserDataSourceMap> GetUserDataSourceMap();
}