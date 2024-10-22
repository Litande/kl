using KL.Statistics.Application.Cache;
using KL.Statistics.Application.Handlers;
using KL.Statistics.Application.Handlers.Agent;
using KL.Statistics.Application.Handlers.Dashboard;
using KL.Statistics.Application.Handlers.LeadStatistics;
using KL.Statistics.Application.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Internal;

namespace KL.Statistics.Configurations;

public static class ServiceConfiguration
{
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        services.AddMemoryCache();

        services.AddScoped(typeof(ICacheService<>), typeof(MemoryCacheService<>));
        services.AddScoped<IStatsMemoryCacheService, StatsMemoryCacheService>();
        services.AddScoped<ICallAnalysisQueryHandler, CallAnalysisQueryHandler>();
        services.AddScoped<ICallAnalysisChangedHandler, CallAnalysisChangedHandler>();
        services.AddScoped<IGetAgentsWorkModeHandler, GetAgentsWorkModeHandler>();
        services.AddScoped<IPerformancePlotQueryHandler, PerformancePlotQueryHandler>();
        services.AddScoped<IPerformanceStatisticQueryHandler, PerformanceStatisticQueryHandler>();
        services.AddScoped<ISenderDashboardPerformanceHandler, SenderDashboardPerformanceHandler>();
        services.AddScoped<ILeadStatisticsChangeHandler, LeadStatisticsChangeHandler>();
        services.AddScoped<ILeadStatisticsQueryHandler, GetLeadStatisticsQueryHandler>();
        services.AddScoped<IAgentStatisticsChangeHandler, AgentStatisticsChangeHandler>();
        services.AddSingleton<ICdrUpdatedService, CdrUpdatedService>();

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.TryAddSingleton<ISystemClock, SystemClock>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IStatisticPeriodService, StatisticPeriodService>();

        return services;
    }

    public static void AddServiceOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<DashboardPerformanceOptions>()
            .Bind(configuration.GetSection(nameof(DashboardPerformanceOptions)));

        services.AddOptions<BackgroundWorkerOptions>()
            .Bind(configuration.GetSection(nameof(BackgroundWorkerOptions)));

        services.AddOptions<NatsPubSubOptions>()
            .Bind(configuration.GetSection("CLIENTS:NatsProviderOptions"));
    }
}