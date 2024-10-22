using KL.Provider.Leads.Application.Models;

namespace KL.Provider.Leads.Persistent.Repositories.Interfaces;

public interface ITimeZoneRepository
{
    IEnumerable<TimeZoneProjection> GetTimeZones();
}