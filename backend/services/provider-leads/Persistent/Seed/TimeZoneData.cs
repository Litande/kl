using System.Text.Json.Serialization;

namespace KL.Provider.Leads.Persistent.Seed;

public class TimeZoneData
{
    [JsonPropertyName("name")] public string CityName { get; set; } = null!;
    [JsonPropertyName("ascii_name")] public string CityNameEn { get; set; } = null!;
    [JsonPropertyName("cou_name_en")] public string? CountryName { get; set; } = null!;
    [JsonPropertyName("country_code")] public string CountryCode { get; set; } = null!;
    [JsonPropertyName("timezone")] public string Timezone { get; set; } = null!;
    [JsonPropertyName("coordinates")] public Coordinate? Coordinates { get; set; }
}

public class Coordinate
{
    [JsonPropertyName("lon")] public double? Longitude { get; set; }
    [JsonPropertyName("lat")] public double? Latitude { get; set; }
}