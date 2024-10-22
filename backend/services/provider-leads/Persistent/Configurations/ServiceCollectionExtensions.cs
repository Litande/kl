using KL.Provider.Leads.Persistent.LeadCallbackHttpClient;
using KL.Provider.Leads.Persistent.LeadProviderHttpClient;
using KL.Provider.Leads.Persistent.Repositories;
using KL.Provider.Leads.Persistent.Repositories.Interfaces;
using KL.Provider.Leads.Persistent.Seed;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;

namespace KL.Provider.Leads.Persistent.Configurations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistent(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddRepositories()
            .AddDbContext(configuration)
            .InitializeDbData();

        services.AddHttpClient(nameof(LeadProviderClient), config => { config.Timeout = TimeSpan.FromSeconds(30); })
            .AddPolicyHandler(GetRetryPolicy());


        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ILeadDataSourceMapRepository, LeadDataSourceMapRepository>();
        services.AddScoped<IUserDataSourceMapRepository, UserDataSourceMapRepository>();
        services.AddScoped<IStatusDataSourceMapRepository, StatusDataSourceMapRepository>();
        services.AddScoped<IDataSourceRepository, DataSourceRepository>();
        services.AddScoped<ILeadProviderClient, LeadProviderClient>();
        services.AddScoped<ILeadRepository, LeadRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<ILeadHistoryRepository, LeadHistoryRepository>();
        services.AddScoped<ISettingsRepository, SettingsRepository>();
        services.AddScoped<ITimeZoneRepository, TimeZoneRepository>();
        services.AddScoped<ILeadCallbackClient, LeadCallbackClient>();

        return services;
    }

    public static IServiceCollection AddDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
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

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.RequestTimeout)
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromMilliseconds(retryAttempt * 100));
    }
}