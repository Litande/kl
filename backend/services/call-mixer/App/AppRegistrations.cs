using KL.Call.Mixer.Services;
using Plat4me.Core.Nats;
using Plat4Me.Core.Storage.Configuration;

namespace KL.Call.Mixer.App;

public static class AppRegistrations
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<GeneralOptions>()
            .Bind(configuration.GetSection("GeneralOptions"));
        services
            .AddOptions<NatsSubjects>()
            .Bind(configuration.GetSection("NatsSubjects"));

        services
            .AddNatsCore(configuration)
            .AddStore(configuration.GetSection("Storage"));

        services
            .AddTransient<IAudioMixerService, AudioMixerService>();

        services.AddHostedService<MixingBackgroundService>();
        return services;
    }
}
