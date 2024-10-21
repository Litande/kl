using Plat4Me.Dial.Statistic.Api.DAL;
using Plat4Me.Dial.Statistic.Api.Workers;

namespace Plat4Me.Dial.Statistic.Api.Configurations;

public static class WorkersConfiguration
{
    public static void AddWorkers(this IServiceCollection services)
    {
        services.AddHostedService<SubscribeHandlersBackgroundService>();
        services.AddHostedService<RedisIndexCreationService>();
        services.AddHostedService<CallAnalysisChangedBackgroundService>();
        services.AddHostedService<DashboardPerformanceBackgroundService>();
        services.AddHostedService<UpdateStatisticsBackgroundService>();
    }
}