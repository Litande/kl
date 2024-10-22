using KL.Engine.Rule.Handlers;
using KL.Engine.Rule.Handlers.Contracts;
using KL.Engine.Rule.Repositories;
using KL.Engine.Rule.RuleEngine;
using KL.Engine.Rule.RuleEngine.MicrosoftEngine;
using KL.Engine.Rule.Services;
using KL.Engine.Rule.Services.Contracts;
using KL.MySql;
using KL.Nats;
using Redis.OM;
using StackExchange.Redis;

namespace KL.Engine.Rule.App;

public static class AppRegistrations
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<LeadProcessingOptions>()
            .Bind(configuration.GetSection(nameof(LeadProcessingOptions)));

        services.AddOptions<ExpirationAgentOptions>()
            .Bind(configuration.GetSection(nameof(ExpirationAgentOptions)));

        services.AddOptions<FutureQueueOptions>()
            .Bind(configuration.GetSection(nameof(FutureQueueOptions)));

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

        services.AddMysql<KlDbContext>(configuration, "lk");

        services
            .AddTransient<IRuleRepository, RuleRepository>()
            .AddTransient<ILeadRepository, LeadRepository>()
            .AddTransient<ILeadQueueRepository, LeadQueueRepository>()
            .AddTransient<ICDRRepository, CDRRepository>()
            .AddTransient<ISettingsRepository, SettingsRepository>()
            .AddTransient<IUserRepository, UserRepository>()
            .AddTransient<ILeadCommentRepository, LeadCommentRepository>();

        services
            .AddTransient<ILeadLastCacheRepository, LeadLastCacheRepository>()
            .AddTransient<IQueueLeadsCacheRepository, QueueLeadsCacheRepository>();

        services
            .AddHostedService<SubscribeHandlersBackgroundService>()
            .AddHostedService<RedisIndexCreationService>()
            .AddHostedService<LeadProcessingBackgroundService>()
            .AddHostedService<AgentAssignationExpiryBackgroundService>();

        return services;
    }
    
     public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        services
            .AddOptions<SubSubjects>()
            .Bind(configuration.GetSection(nameof(SubSubjects)));

        services
            .AddOptions<PubSubjects>()
            .Bind(configuration.GetSection(nameof(PubSubjects)));

        services.AddHttpContextAccessor();

        services.AddNatsCore(configuration);

        services
            .AddTransient<ISetLeadManualScoreHandler, SetLeadManualScoreHandler>()
            .AddTransient<IGetNextFromLeadQueueHandler, GetNextFromLeadQueueHandler>()
            .AddTransient<ILeadQueueStoreUpdateHandler, LeadQueueStoreUpdateHandler>()
            .AddTransient<ILeadsQueueUpdateNotificationHandler, LeadsQueueUpdateNotificationHandler>()
            .AddTransient<IImportedProcessingService, ImportedProcessingService>()
            .AddTransient<ILeadProcessingImported, LeadProcessingImported>()
            .AddTransient<ILeadsImportedHandler, LeadsImportedHandler>()
            .AddTransient<ILeadFeedbackFilledHandler, LeadFeedbackFilledHandler>()
            .AddTransient<ILeadFeedbackCallFailedHandler, LeadFeedbackCallFailedHandler>()
            .AddTransient<ILeadBlockedHandler, LeadBlockedHandler>();

        services
            .AddTransient<ILeadProcessingPipeline, LeadProcessingPipeline>()
            .AddTransient<IQueueProcessingService, QueueProcessingService>()
            .AddTransient<IScoreProcessingService, ScoreProcessingService>()
            .AddTransient<IBehaviourProcessingService, BehaviourProcessingService>()
            .AddTransient<IRuleEngineProcessingService, MicrosoftRuleEngineProcessingService>()
            .AddTransient<IAgentsAssignationExpiryProcessing, AgentsAssignationExpiryProcessing>();

        services.AddScoped<ILeadQueueRuleService, LeadQueueRuleService>();

        services
            .AddSingleton<ILeadsQueueStore, LeadsQueueStore>()
            .AddSingleton<IEngineMapper, MicrosoftEngineMapper>();

        return services;

    }
}