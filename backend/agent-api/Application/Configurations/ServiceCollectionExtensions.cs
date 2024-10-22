using KL.Agent.API.Application.Handlers;
using KL.Agent.API.Application.Services;
using KL.Agent.API.Middlewares;
using KL.Agent.API.SignalR;
using KL.Nats;
using KL.Storage.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace KL.Agent.API.Application.Configurations;

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
            .AddOptions(configuration);

        return services;
    }

    public static IServiceCollection AddNATs(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<NatsPubSubOptions>()
            .Bind(configuration.GetSection("CLIENTS:NatsProviderOptions"));

        services.AddNatsCore(configuration);

        return services;
    }

    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        services
            .AddScoped<ICallInfoHandler, CallInfoHandler>()
            .AddScoped<IAgentCurrentStatusHandler, AgentCurrentStatusHandler>()
            .AddScoped<IAgentFilledCallInfoHandler, AgentFilledCallInfoHandler>()
            .AddScoped<IAgentConnectedHandler, AgentConnectedHandler>()
            .AddScoped<IAgentDisconnectedHandler, AgentDisconnectedHandler>()
            .AddScoped<IManualCallHandler, ManualCallHandler>()
            .AddScoped<IAgentChangeStatusRequestHandler, AgentChangeStatusRequestHandler>()
            .AddScoped<ICallAgainHandler, CallAgainHandler>()
            .AddScoped<IFeedbackTimeoutHandler, FeedbackTimeoutHandler>()
            .AddScoped<IUserQueryHandler, UserQueryHandler>()
            .AddScoped<IAgentBlockedHandler, AgentBlockedHandler>()
            .AddScoped<IDownloadAudioRecordHandler, DownloadAudioRecordHandler>()
            .AddScoped<IAgentChangePasswordHandler, AgentChangePasswordHandler>()
            .AddScoped<IAgentStateService, AgentStateService>()
            .AddScoped<ICallMessagesHandler, CallMessagesHandler>();

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .AddSingleton<IAgentTimeoutService, AgentTimeoutService>();

        services
            .AddScoped<IHubSender, HubSender>();

        services.AddSingleton<
                IAuthorizationMiddlewareResultHandler,
                BlockedUserAuthorizationMiddlewareHandler>();

        return services;
    }

    public static void AddOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<AgentHubOptions>()
            .Bind(configuration.GetSection("AgentHub"));

        services
            .AddOptions<GeneralOptions>()
            .Bind(configuration.GetSection("GeneralOptions"));
    }
}
