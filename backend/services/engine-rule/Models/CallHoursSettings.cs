namespace KL.Engine.Rule.Models;

public record CallHoursSettings(string Country, CallHours[] CallHours);
public record CallHours(TimeSpan From, TimeSpan Till, string Country);