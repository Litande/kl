using KL.Agent.API.Persistent.Repositories;
using KL.Agent.API.Persistent.Repositories.Interfaces;
using KL.Agent.API.Persistent.Seed;
using KL.MySql;
using Medallion.Threading;
using Medallion.Threading.Redis;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;
using Polly.Contrib.DuplicateRequestCollapser;
using Redis.OM;
using StackExchange.Redis;

namespace KL.Agent.API.Persistent.Configurations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistent(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddDbContext(configuration)
            .InitializeDbData();

        services
            .AddMemoryCacheProvider()
            .AddRepositories()
            .AddCacheRepositories()
            .AddRedis(configuration);

        return services;
    }

    public static IServiceCollection AddMemoryCacheProvider(this IServiceCollection services)
    {
        services
            .AddMemoryCache();

        services
            .AddSingleton<IAsyncCacheProvider, MemoryCacheProvider>()
            .AddSingleton<IAsyncPolicy, AsyncPolicy>(serviceProvider =>
            {
                var memoryCacheProvider = serviceProvider.GetRequiredService<IAsyncCacheProvider>();
                var memoryCachePolicy = Policy.CacheAsync(memoryCacheProvider, TimeSpan.FromMinutes(2));
                return AsyncRequestCollapserPolicy
                    .Create()
                    .WrapAsync(memoryCachePolicy);
            });

        return services;
    }

    public static IServiceCollection AddDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMysql<KlDbContext>(configuration, "kl");
        
        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services
            .AddScoped<ILeadRepository, LeadRepository>()
            .AddScoped<IRuleRepository, RuleRepository>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<ICDRRepository, CDRRepository>()
            .AddScoped<ICommonsRepository, CommonsRepository>()
            .AddScoped<ISettingsRepository, SettingsRepository>()
            .AddScoped<ILeadCommentRepository, LeadCommentRepository>()
            .AddScoped<IAgentStatusHistoryRepository, AgentStatusHistoryRepository>();

        return services;
    }

    public static IServiceCollection AddCacheRepositories(this IServiceCollection services)
    {
        services
            .AddScoped<ILeadCacheRepository, LeadCacheRepository>()
            .AddScoped<IAgentCacheRepository, AgentCacheRepository>()
            .AddScoped<ICallInfoCacheRepository, CallInfoCacheRepository>()
            .AddScoped<IBlockedUserCacheRepository, BlockedUserCacheRepository>();

        return services;
    }

    public static IServiceCollection AddRedis(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var redisOptions = new RedisOptions
        {
            Host = configuration.GetValue<string>("CLIENTS:Redis:HOST"),
            Port = configuration.GetValue<string>("CLIENTS:Redis:PORT"),
        };

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        ConnectionMultiplexer.Connect(new ConfigurationOptions
        {
            EndPoints = new EndPointCollection { $"{redisOptions.Host}:{redisOptions.Port}" },
            ReconnectRetryPolicy = new LinearRetry((int)TimeSpan.FromSeconds(1).TotalMilliseconds),
        })
       );

        services.AddSingleton<RedisConnectionProvider>(sp =>
            new RedisConnectionProvider(sp.GetRequiredService<IConnectionMultiplexer>())
        );

        services.AddSingleton<IDistributedLockProvider>(sp =>
            new RedisDistributedSynchronizationProvider(sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase()));

        return services;
    }
}
