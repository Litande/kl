namespace Plat4Me.DialClientApi.Persistent.Configurations;

public class RuleEngineClientOptions
{
    public string BaseUrl { get; set; } = null!;
    public string ValidateRules { get; set; } = null!;
    public string Conditions { get; set; } = null!;
    public string Actions { get; set; } = null!;
}
