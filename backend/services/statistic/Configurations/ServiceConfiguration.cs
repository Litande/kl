using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Internal;
using Plat4Me.Dial.Statistic.Api.Application.Cache;
using Plat4Me.Dial.Statistic.Api.Application.Handlers;
using Plat4Me.Dial.Statistic.Api.Application.Handlers.Agent;
using Plat4Me.Dial.Statistic.Api.Application.Handlers.Dashboard;
using Plat4Me.Dial.Statistic.Api.Application.Handlers.LeadStatistics;
using Plat4Me.Dial.Statistic.Api.Application.Services;

namespace Plat4Me.Dial.Statistic.Api.Configurations;

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