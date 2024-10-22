using KL.RabbitMq.Configurations;
using KL.RabbitMq.Publisher;
using KL.RabbitMq.RetryPolicies;
using KL.RabbitMq.Subscriber;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace KL.RabbitMq.Messaging;

public static class Configuration
{
    public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfigurationSection section)
    {
        services.Configure<RabbitMqOptions>(section);
        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
            return new ConnectionFactory
            {
                UserName = options.User,
                Password = options.Pass,
                HostName = options.Host,
                Port = options.Port,
                DispatchConsumersAsync = true

        };
        });
        services.AddSingleton<IRabbitMqPersistentConnection, RabbitMqPersistentConnection>();
        services.AddSingleton<RetryPolicyFactory>();

        return services;
    }

    public static IServiceCollection AddPublisher<T>(this IServiceCollection services, IConfigurationSection section)
    {
        var optionSection = section.GetSection($"Publisher:{typeof(T).Name}");

        services.Configure<MessagePublisherConfiguration<T>>(optionSection);

        services.AddSingleton<IMessagingPublisher<T>, RabbitMqMessagingPublisher<T>>();

        return services;
    }

    public static IServiceCollection AddSubscriber<T>(this IServiceCollection services, IConfigurationSection section)
    {
        var optionSection = section.GetSection($"Subscriber:{typeof(T).Name}");

        services.Configure<MessageSubscriberConfiguration<T>>(optionSection);

        services.AddSingleton<IMessagingSubscriber<T>, RabbitMqMessagingSubscriber<T>>();

        return services;
    }
}
