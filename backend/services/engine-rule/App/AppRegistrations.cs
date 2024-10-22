using KL.Engine.Rule.Repositories;

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

        var mysqlOptions = new MysqlOptions
        {
            Host = configuration.GetValue<string>("CLIENTS:MYSQL:HOST"),
            Pass = configuration.GetValue<string>("CLIENTS:MYSQL:PASS"),
            User = configuration.GetValue<string>("CLIENTS:MYSQL:USER"),
            Port = configuration.GetValue<string>("CLIENTS:MYSQL:PORT"),
        };

        var connectionString = mysqlOptions.GetUrl("dial");
        services.AddDbContextFactory<DialDbContext>(options => options.UseMySql(connectionString,
            ServerVersion.AutoDetect(connectionString),
            opt => opt.EnableRetryOnFailure(mysqlOptions.ConnectRetry)));

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