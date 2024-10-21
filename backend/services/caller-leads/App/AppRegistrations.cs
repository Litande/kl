using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Plat4me.Core.Nats;
using Plat4Me.DialLeadCaller.Application.Handlers;
using Plat4Me.DialLeadCaller.Application.Handlers.Contracts;
using Plat4Me.DialLeadCaller.Application.Services;
using Plat4Me.DialLeadCaller.Application.Services.Contracts;

namespace Plat4Me.DialLeadCaller.Application.App;

public static class AppRegistrations
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<CallerBackgroundOptions>()
            .Bind(configuration.GetSection(nameof(CallerBackgroundOptions)));

        services
            .AddOptions<LeadQueueClientOptions>()
            .Bind(configuration.GetSection(nameof(LeadQueueClientOptions)));

        services
            .AddOptions<BridgeClientOptions>()
            .Bind(configuration.GetSection(nameof(BridgeClientOptions)));

        services
            .AddOptions<DropRateBackgroundOptions>()
            .Bind(configuration.GetSection(nameof(DropRateBackgroundOptions)));

        services
            .AddOptions<LeadStatisticProcessingOptions>()
            .Bind(configuration.GetSection(nameof(LeadStatisticProcessingOptions)));

        services
            .AddOptions<CallPublishBackgroundOptions>()
            .Bind(configuration.GetSection(nameof(CallPublishBackgroundOptions)));


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

        var mysqlOptions = new MysqlOptions
        {
            Host = configuration.GetValue<string>("CLIENTS:MYSQL:HOST"),
            Pass = configuration.GetValue<string>("CLIENTS:MYSQL:PASS"),
            User = configuration.GetValue<string>("CLIENTS:MYSQL:USER"),
            Port = configuration.GetValue<string>("CLIENTS:MYSQL:PORT"),
        };

        var connectionString = mysqlOptions.GetUrl("dial");
        services.AddDbContext<DialDbContext>(options => options.UseMySql(connectionString,
            ServerVersion.AutoDetect(connectionString),
            opt => opt.EnableRetryOnFailure(mysqlOptions.ConnectRetry)));

        services.AddHttpClient(nameof(LeadRuleEngineClient), config =>
        {
            config.Timeout = new TimeSpan(0, 0, 30);
        }).AddPolicyHandler(GetRetryPolicy());

        services
            .AddTransient<ILeadQueueRepository, LeadQueueRepository>()
            .AddTransient<ILeadRepository, LeadRepository>()
            .AddTransient<IAgentRepository, AgentRepository>()
            .AddTransient<ICDRRepository, CDRRepository>()
            .AddTransient<IUserRepository, UserRepository>()
            .AddTransient<ISettingsRepository, SettingsRepository>()
            .AddTransient<ISipProviderRepository, SipProviderRepository>();

        services
            .AddTransient<ICallInfoCacheRepository, CallInfoCacheRepository>()
            .AddTransient<IAgentStateRepository, AgentStateRepository>()
            .AddTransient<ILeadCacheRepository, LeadCacheRepository>()
            .AddTransient<IQueueLeadsCacheRepository, QueueLeadsCacheRepository>()
            .AddTransient<IQueueDropRateCacheRepository, QueueDropRateCacheRepository>()
            .AddTransient<ILeadStatisticCacheRepository, LeadStatisticCacheRepository>();

        services
            .AddTransient<ILeadRuleEngineClient, LeadRuleEngineClient>()
            .AddTransient<IBridgeClient, BridgeClient>();

        services.AddHostedService<SubscribeHandlersBackgroundService>();
        services.AddHostedService<RedisIndexCreationService>();
        services.AddHostedService<CallerLoopBackgroundService>();
        services.AddHostedService<DropRateBackgroundService>();
        services.AddHostedService<LeadStatisticBackgroundService>();

        services.AddSingleton<CallPublishBackgroundService>();
        services.AddSingleton<ICallPublishService>(serviceProvider => serviceProvider.GetRequiredService<CallPublishBackgroundService>());
        services.AddHostedService<CallPublishBackgroundService>(serviceProvider => serviceProvider.GetRequiredService<CallPublishBackgroundService>());

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
    
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMemoryCache();

        services
            .AddOptions<SubSubjects>()
            .Bind(configuration.GetSection(nameof(SubSubjects)));

        services
            .AddOptions<PubSubjects>()
            .Bind(configuration.GetSection(nameof(PubSubjects)));

        services
            .AddOptions<LeadOptions>()
            .Bind(configuration.GetSection(nameof(LeadOptions)));

        services
            .AddOptions<DialerOptions>()
            .Bind(configuration.GetSection(nameof(DialerOptions)));

        services.AddNatsCore(configuration);

        services
            .AddTransient<IPublishForCallHandler, PublishForCallHandler>()
            .AddTransient<IAgentNotAnsweredHandler, AgentNotAnsweredHandler>()
            .AddTransient<ICallFailedHandler, CallFailedHandler>()
            .AddTransient<ICallFinishedHandler, CallFinishedHandler>()
            .AddTransient<ICallFinishedRecordsHandler, CallFinishedRecordsHandler>()
            .AddTransient<IAgentAnsweredHandler, AgentAnsweredHandler>()
            .AddTransient<ILeadAnsweredHandler, LeadAnsweredHandler>()
            .AddTransient<ILeadFeedbackFilledHandler, LeadFeedbackFilledHandler>()
            .AddTransient<IMixedRecordReadyHandler, MixedRecordReadyHandler>()
            .AddTransient<IManualCallHandler, ManualCallHandler>()
            .AddTransient<ICallAgainHandler, CallAgainHandler>()
            .AddTransient<IBridgeRunHandler, BridgeRunHandler>()
            .AddTransient<IDroppedAgentHandler, DroppedAgentHandler>()
            .AddTransient<IAgentFilledCallInfoHandler, AgentFilledCallInfoHandler>()
            .AddTransient<IEnqueueAgentForCallHandler, EnqueueAgentForCallHandler>()
            .AddTransient<IDequeueAgentForCallHandler, DequeueAgentForCallHandler>();

        services
            .AddTransient<ICallerService, CallerService>()
            .AddTransient<IDropRateService, DropRateService>()
            .AddTransient<ILeadStatisticProcessing, LeadStatisticProcessingService>()
            .AddTransient<ISettingsService, SettingsService>()
            .AddTransient<ICDRService, CDRService>()
            .AddTransient<ISipProviderService, SipProviderService>()
            .AddTransient<ICallInfoService, CallInfoService>();

        services
            .AddSingleton<IBridgeService, BridgeService>();

        return services;
    }
}
