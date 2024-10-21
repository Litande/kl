using Plat4Me.DialCallRecordMixer.Workers;
using Plat4me.Core.Nats;
using Plat4Me.Core.Storage.Configuration;
using Plat4Me.DialCallRecordMixer.Services;

namespace Plat4Me.DialCallRecordMixer.App;

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
