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

        var mysqlOptions = new SqlOptions
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

        return services;
    }
}