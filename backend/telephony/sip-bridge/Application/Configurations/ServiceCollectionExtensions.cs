using KL.Nats;
using KL.SIP.Bridge.Application.Handlers;
using KL.SIP.Bridge.Application.Services;
using KL.SIP.Bridge.Application.Session;
using KL.SIP.Bridge.Application.Workers;
using KL.Storage.Configuration;

namespace KL.SIP.Bridge.Application.Configurations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddNATs(configuration)
            .AddStore(configuration.GetSection("Storage"))
            .AddHandlers()
            .AddServices()
            .AddWorkers()
            .AddOptions(configuration);

        return services;
    }

    public static IServiceCollection AddNATs(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        services
            .AddOptions<NatsSubjects>()
            .Bind(configuration.GetSection("NatsSubjects"));

        services.AddNatsCore(configuration);

        return services;
    }

    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        services
            .AddTransient<IBridgeRegRequestHandler, BridgeRegRequestHandler>()
            .AddTransient<ICallToLeadHandler, CallToLeadHandler>()
            .AddTransient<IHangupCallHandler, HangupCallHandler>()
            .AddTransient<IAgentReplaceDataHandler, AgentReplaceDataHandler>();

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
         services
            .AddTransient<CallSessionBuilder, CallSessionBuilder>()
            .AddScoped<ISessionRecordingService, SessionRecordingService>()
             .AddSingleton<ICallService, CallService>();

        return services;
    }

    public static void AddOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<GeneralOptions>()
            .Bind(configuration.GetSection("GeneralOptions"));

        services
            .AddOptions<RTCOptions>()
            .Bind(configuration.GetSection("RTCOptions"));

        services
            .AddOptions<SIPOptions>()
            .Bind(configuration.GetSection("SIPOptions"));

        services
            .AddOptions<CallSessionOptions>()
            .Bind(configuration.GetSection("SessionOptions"));


        services
            .AddOptions<FakeCallOptions>()
            .Bind(configuration.GetSection("FakeCall"));      
        
        services
            .AddOptions<JwtOptions>()
            .Bind(configuration.GetSection("Jwt"));
     
    }

    public static IServiceCollection AddWorkers(
        this IServiceCollection services)
    {
        services.AddSingleton<UploaderBackgroundService>();
        services.AddSingleton<IUploaderService, UploaderBackgroundService>(p => p.GetRequiredService<UploaderBackgroundService>());

        services
            .AddHostedService<SubscribeHandlersBackgroundService>()
            .AddHostedService<RTCBackgroundService>()
            .AddHostedService<UploaderBackgroundService>(p => p.GetRequiredService<UploaderBackgroundService>());

        return services;
    }
}
