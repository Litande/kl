namespace Plat4Me.DialSipBridge.Application.Configurations;

public class CallSessionOptions
{
    public string AccessUrl { get; set; } = null!;
    public PredictiveTimeouts PredictiveTimeouts { get; set; } = null!;
    public ManualTimeouts ManualTimeouts { get; set; } = null!;
}

public class PredictiveTimeouts
{
    public long SessionTimeout { get; set; } = 20000;
    public long AgentTimeout { get; set; } = 20000;
    public long AgentReconnectTimeout { get; set; } = 20000;

}

public class ManualTimeouts
{
    public long SessionTimeout { get; set; } = 20000;
    public long LeadTimeout { get; set; } = 20000;
    public long AgentReconnectTimeout { get; set; } = 20000;

}
