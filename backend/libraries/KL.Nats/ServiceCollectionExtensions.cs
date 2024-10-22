using AlterNats;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KL.Nats;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNatsCore(this IServiceCollection services, IConfiguration config) =>
        services
            .AddNats(configureOptions: _ => AlterNats.NatsOptions.Default with
            {
                Url = $"nats://{config.GetValue<string>("CLIENTS:NATS:HOST")}:{config.GetValue<string>("CLIENTS:NATS:PORT")}",
                LoggerFactory = new MinimumConsoleLoggerFactory(LogLevel.Warning)
            })
            .AddSingleton<INatsPublisher, NatsPublisher>()
            .AddSingleton<INatsSubscriber, NatsSubscriber>();
}
