namespace Plat4Me.DialLeadProvider.Persistent.Entities;

public class TimeZone
{
    public long Id { get; set; }
    public string CityName { get; set; } = null!;
    public string CityNameEn { get; set; } = null!;
    public string? CountryName { get; set; } = null!;
    public string CountryCode { get; set; } = null!;
    public string Timezone { get; set; } = null!;
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
}