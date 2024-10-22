using KL.Nats;
using KL.Provider.Leads.Application.Handlers;
using KL.Provider.Leads.Application.Handlers.Interfaces;
using KL.Provider.Leads.Application.Services;
using KL.Provider.Leads.Application.Services.Interfaces;
using KL.Provider.Leads.Workers;

namespace KL.Provider.Leads.Application.Configurations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddNatsCore(configuration)
            .AddHandlers()
            .AddServices()
            .AddOptions(configuration)
            .AddWorkers();

        return services;
    }

    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        services.AddScoped<ILeadFeedbackProcessedHandler, LeadFeedbackProcessedHandler>();
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IDataSourceMapService, DataSourceMapService>();
        services.AddScoped<ILeadsDataSourceSync, LeadsDataSourceSync>();
        services.AddScoped<ILeadDataMapperService, LeadDataMapperService>();

        return services;
    }

    public static IServiceCollection AddOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<LeadProviderOptions>()
            .Bind(configuration.GetSection(nameof(LeadProviderOptions)));

        services.AddOptions<PubSubjects>()
            .Bind(configuration.GetSection(nameof(PubSubjects)));
        
        services
            .AddOptions<SubSubjects>()
            .Bind(configuration.GetSection(nameof(SubSubjects)));

        return services;
    }

    public static IServiceCollection AddWorkers(this IServiceCollection services)
    {
        services.AddHostedService<LeadProviderBackgroundService>();
        services.AddHostedService<NatsListenerBackgroundService>();

        return services;
    }
}