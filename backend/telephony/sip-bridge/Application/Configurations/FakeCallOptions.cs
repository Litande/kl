namespace KL.SIP.Bridge.Application.Configurations;

public class FakeCallOptions
{
    public long MinAnswerDelay { get; set; } = 2000;
    public long MaxAnswerDelay { get; set; } = 10000;
    public string[] DemoFiles { get; set; } = null!;
}
