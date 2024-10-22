namespace KL.Auth.Services.Identity
{
    public class JwtConfigurations
    {
        public string Key { get; set; }

        public int ExpirationInDays { get; set; }
    }
}