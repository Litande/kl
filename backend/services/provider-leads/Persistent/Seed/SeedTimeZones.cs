using System.Text.Json;
using TimeZone = KL.Provider.Leads.Persistent.Entities.TimeZone;

namespace KL.Provider.Leads.Persistent.Seed;

public static class SeedTimeZones
{
    public static void Seed(DialDbContext context)
    {
        AddTimeZonesIfNeed(context);
    }

    private static void AddTimeZonesIfNeed(DialDbContext context)
    {
        var isTimeZoneExist = context.TimeZones.Any();
        if (isTimeZoneExist) return;

        var timeZoneData = File.ReadAllText("Seed/Data/timezones.json");
        var timeZones = JsonSerializer.Deserialize<IReadOnlyCollection<TimeZoneData>>(timeZoneData);
        var entityTimeZones = timeZones?.Select(t => new TimeZone()
        {
            CityName = t.CityName,
            CityNameEn = t.CityNameEn,
            CountryCode = t.CountryCode,
            CountryName = t.CountryName,
            Timezone = t.Timezone,
            Latitude = t.Coordinates?.Latitude,
            Longitude = t.Coordinates?.Longitude
        });

        if (entityTimeZones == null) return;
        context.TimeZones.AddRange(entityTimeZones);
        context.SaveChanges();
    }
}