namespace KL.SIP.Bridge.Application.App;

public class JwtOptions
{
    public string Key { get; set; } = null!;
    public int ExpirationInDays { get; set; }
}
