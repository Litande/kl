﻿using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Internal;
using Plat4me.Core.Nats;
using Plat4Me.Core.Storage.Configuration;
using Plat4Me.DialClientApi.Application.Handlers;
using Plat4Me.DialClientApi.Application.Handlers.Agents;
using Plat4Me.DialClientApi.Application.Handlers.LeadGroups;
using Plat4Me.DialClientApi.Application.Handlers.Leads;
using Plat4Me.DialClientApi.Application.Handlers.LiveTracking;
using Plat4Me.DialClientApi.Application.Services;
using Plat4Me.DialClientApi.Application.Services.Interfaces;
using Plat4Me.DialClientApi.SignalR;

namespace Plat4Me.DialClientApi.Application.Configurations;

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
            .AddScoped<IStatusQueryHandler, GetStatusQueryHandler>()
            .AddScoped<IQueuesUpdatedHandler, QueuesUpdatedHandler>()
            .AddScoped<IAgentsQueryHandler, GetAgentsQueryHandler>()
            .AddScoped<IAgentChangedStatusHandler, AgentChangedStatusHandler>()
            .AddScoped<ILeadsQueueQueryHandler, LeadsQueueQueryHandler>()
            .AddScoped<ILeadInfoQueryHandler, LeadInfoQueryHandler>()
            .AddScoped<ILeadQueueUpdateRatioHandler, LeadQueueUpdateRatioHandler>()
            .AddScoped<IUserQueryHandler, UserQueryHandler>()
            .AddScoped<ICallFinishByManagerHandler, CallFinishByManagerHandler>()
            .AddScoped<IRuleEngineRunHandler, RuleEngineRunHandler>()
            .AddScoped<IBlockedAgentHandler, BlockedAgentHandler>()
            .AddScoped<IDownloadAudioRecordHandler, DownloadAudioRecordHandler>()
            .AddScoped<IAgentChangePasswordHandler, AgentChangePasswordHandler>()
            .AddScoped<IBlockLeadHandler, LeadBlockHandler>()
            ;

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .TryAddSingleton<ISystemClock, SystemClock>();

        services
            .AddScoped<IHubSender, HubSender>()
            .AddScoped<IRuleService, RuleService>()
            .AddScoped<IRuleGroupService, RuleGroupService>()
            .AddScoped<IDashboardService, DashboardService>()
            .AddScoped<IStatisticPeriodService, StatisticPeriodService>();

        return services;
    }

    public static void AddOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<GeneralOptions>()
            .Bind(configuration.GetSection("GeneralOptions"));
    }
}
