using KL.Provider.Leads.Application.Models;
using KL.Provider.Leads.Persistent.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KL.Provider.Leads.Persistent.Repositories;

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