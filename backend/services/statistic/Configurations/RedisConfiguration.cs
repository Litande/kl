using Redis.OM;
using StackExchange.Redis;

namespace KL.Statistics.Configurations;

public static class RedisConfiguration
{
    public static void AddRedisConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var redisOptions = new RedisOptions
        {
            Host = configuration.GetValue<string>("CLIENTS:Redis:HOST"),
            Port = configuration.GetValue<string>("CLIENTS:Redis:PORT"),
        };

        services.AddSingleton(new RedisConnectionProvider(
                ConnectionMultiplexer.Connect(new ConfigurationOptions
                    {
                        EndPoints = new EndPointCollection { $"{redisOptions.Host}:{redisOptions.Port}" },
                        ReconnectRetryPolicy = new LinearRetry((int)TimeSpan.FromSeconds(1).TotalMilliseconds),
                    }
                )
            )
        );
    }
}