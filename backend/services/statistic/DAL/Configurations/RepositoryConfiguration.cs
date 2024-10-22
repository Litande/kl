using KL.Statistics.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KL.Statistics.DAL.Configurations;

public static class RepositoryConfiguration
{
    public static IServiceCollection AddRepositories(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<IAgentCacheRepository, AgentCacheRepository>();
        services.AddTransient<IClientRepository, ClientRepository>();
        services.AddTransient<ICdrRepository, CdrRepository>();
        services.AddTransient<ILeadQueueRepository, LeadQueueRepository>();
        services.AddTransient<IQueueLeadsCacheRepository, QueueLeadsCacheRepository>();
        services.AddTransient<IQueueDropRateCacheRepository, QueueDropRateCacheRepository>();
        services.AddTransient<ILeadStatisticCacheRepository, LeadStatisticCacheRepository>();

        return services;
    }
}