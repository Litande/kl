using Plat4Me.DialLeadProvider.Application.Models;

namespace Plat4Me.DialLeadProvider.Persistent.Repositories.Interfaces;

public interface ITimeZoneRepository
{
    IEnumerable<TimeZoneProjection> GetTimeZones();
}