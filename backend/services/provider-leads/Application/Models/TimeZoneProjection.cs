﻿namespace KL.Provider.Leads.Application.Models;

public class TimeZoneProjection
{
    public string CityName { get; set; } = null!;
    public string CountryCode { get; set; } = null!;
    public string Timezone { get; set; } = null!;
}