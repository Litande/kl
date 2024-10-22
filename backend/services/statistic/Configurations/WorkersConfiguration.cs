using KL.Statistics.DAL;
using KL.Statistics.Workers;

namespace KL.Statistics.Configurations;

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