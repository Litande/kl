using System.Text.Json.Serialization;

namespace Plat4Me.DialClientApi.Application.Models.Requests.Click2Call;

public class SettingsValue<T>
{
    public string Text { get; set; } = null!;
    public T Value { get; set; } = default(T)!;
}

public class CallSettings
{
    public SettingsValue<string>? PhoneOption { get; set; }
    public SettingsValue<int>? ProviderConfig { get; set; }
    public string? CallerOption { get; set; }
}

public class CallRequest
{
    [JsonPropertyName("trader_id")]
    public long TraderId { get; set; }
    [JsonPropertyName("settings")]
    public CallSettings? Settings { get; set; }
}
