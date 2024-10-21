using Plat4Me.DialStun.Workers;

namespace Plat4Me.DialStun.App;

public static class AppRegistrations
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<StunOptions>()
            .Bind(configuration.GetSection("StunOptions"));

        services.AddHostedService<StunBackgroundService>();
        return services;
    }
}
