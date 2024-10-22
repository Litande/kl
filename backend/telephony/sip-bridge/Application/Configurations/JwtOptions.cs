namespace KL.SIP.Bridge.Application.Configurations;

public class JwtOptions
{
    public string Key { get; set; } = null!;
    public int ExpirationInDays { get; set; }
}
