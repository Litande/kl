namespace Plat4Me.DialSipBridge.Infrastructure.App;

public class JwtOptions
{
    public string Key { get; set; } = null!;
    public int ExpirationInDays { get; set; }
}
