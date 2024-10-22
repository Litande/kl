using System.Net;
using KL.Manager.API.Persistent.Clients;
using KL.Manager.API.Persistent.Repositories;
using KL.Manager.API.Persistent.Repositories.Interfaces;
using KL.Manager.API.Persistent.Seed;
using KL.MySql;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;
using Polly.Contrib.DuplicateRequestCollapser;
using Polly.Extensions.Http;
using Redis.OM;
using StackExchange.Redis;

namespace KL.Manager.API.Persistent.Configurations;

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
            .AddRedis(configuration)
            .AddRuleEngineClient(configuration);

        return services;
    }

    public static IServiceCollection AddDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMysql<KlDbContext>(configuration, "kl");
        
        
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

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IRuleEngineClient, RuleEngineClient>();

        services
            .AddScoped<ILeadRepository, LeadRepository>()
            .AddScoped<ILeadQueueRepository, LeadQueueRepository>()
            .AddScoped<IRuleRepository, RuleRepository>()
            .AddScoped<IRuleGroupRepository, RuleGroupRepository>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<ICommonsRepository, CommonsRepository>()
            .AddScoped<ISettingsRepository, SettingsRepository>()
            .AddScoped<ITagRepository, TagRepository>()
            .AddScoped<ICDRRepository, CDRRepository>()
            .AddScoped<ITeamRepository, TeamRepository>()
            .AddScoped<IClientRepository, ClientRepository>()
            .AddScoped<ILeadBlacklistRepository, LeadBlacklistRepository>()
            .AddScoped<IUserFilterPreferencesRepository, UserFilterPreferencesRepository>()
            .AddScoped<IDataSourceRepository, DataSourceRepository>();

        services
            .AddSingleton<IRuleEngineCacheRepository, RuleEngineCacheRepository>();

        return services;
    }

    public static IServiceCollection AddCacheRepositories(this IServiceCollection services)
    {
        services
            .AddScoped<IAgentCacheRepository, AgentCacheRepository>()
            .AddScoped<ICallInfoCacheRepository, CallInfoCacheRepository>()
            .AddScoped<ILeadStatisticCacheRepository, LeadStatisticCacheRepository>()
            .AddScoped<ILeadLastCacheRepository, LeadLastLastCacheRepository>()
            .AddScoped<IQueueLeadsCacheRepository, QueueLeadsCacheRepository>()
            .AddScoped<IQueueDropRateCacheRepository, QueueDropRateCacheRepository>()
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

        // services.AddSingleton(new RedisConnectionProvider(redisOptions.GetConfiguration()));
        services.AddSingleton(new RedisConnectionProvider(
                ConnectionMultiplexer.Connect(new ConfigurationOptions
                {
                    EndPoints = new EndPointCollection { $"{redisOptions.Host}:{redisOptions.Port}" },
                    ReconnectRetryPolicy = new LinearRetry((int)TimeSpan.FromSeconds(1).TotalMilliseconds),
                }
                )
            )
        );

        return services;
    }

    public static IServiceCollection AddRuleEngineClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<RuleEngineClientOptions>()
            .Bind(configuration.GetSection(nameof(RuleEngineClientOptions)));

        services
            .AddHttpClient(nameof(RuleEngineClient), config => { config.Timeout = new TimeSpan(0, 0, 30); })
            .AddPolicyHandler(GetRetryPolicy());

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.RequestTimeout)
            .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromMilliseconds(retryAttempt * 100));
    }
}
