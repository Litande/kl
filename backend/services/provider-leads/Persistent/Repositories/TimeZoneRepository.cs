using Microsoft.EntityFrameworkCore;
using Plat4Me.DialLeadProvider.Application.Models;
using Plat4Me.DialLeadProvider.Persistent.Repositories.Interfaces;

namespace Plat4Me.DialLeadProvider.Persistent.Repositories;

public class TimeZoneRepository : ITimeZoneRepository
{
    private readonly DialDbContext _context;

    public TimeZoneRepository(DialDbContext context)
    {
        _context = context;
    }

    public IEnumerable<TimeZoneProjection> GetTimeZones()
    {
        var timeZones = _context.TimeZones.AsNoTracking()
            .Select(t => new TimeZoneProjection
            {
                CityName = t.CityName,
                CountryCode = t.CountryCode,
                Timezone = t.Timezone
            }).ToArray();

        return timeZones;
    }
}