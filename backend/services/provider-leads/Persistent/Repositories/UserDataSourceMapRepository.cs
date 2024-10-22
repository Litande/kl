using KL.Provider.Leads.Persistent.Entities;
using KL.Provider.Leads.Persistent.Repositories.Interfaces;

namespace KL.Provider.Leads.Persistent.Repositories;

public class UserDataSourceMapRepository : IUserDataSourceMapRepository
{
    private readonly KlDbContext _context;

    public UserDataSourceMapRepository(KlDbContext context)
    {
        _context = context;
    }

    public IEnumerable<UserDataSourceMap> GetUserDataSourceMap()
    {
        var userDataSourceMap = _context.UserDataSourceMaps
            .ToList();

        return userDataSourceMap;
    }
}