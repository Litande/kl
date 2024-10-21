using Plat4Me.DialLeadProvider.Persistent.Entities;
using Plat4Me.DialLeadProvider.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialLeadProvider.Persistent.Repositories;

public class UserDataSourceMapRepository : IUserDataSourceMapRepository
{
    private readonly DialDbContext _context;

    public UserDataSourceMapRepository(DialDbContext context)
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