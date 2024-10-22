using System.Text.Json.Serialization;
using KL.Agent.API.Workers;
using Microsoft.OpenApi.Models;

namespace KL.Agent.API.Configurations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomControllers(this IServiceCollection services)
    {
        services
            .AddControllers()
            .AddJsonOptions(opt => opt.JsonSerializerOptions.Converters
                .Add(new JsonStringEnumConverter()));

        return services;
    }

    public static IServiceCollection AddCustomSignalR(this IServiceCollection services)
    {
        services
            .AddSignalR()
            .AddJsonProtocol(opt => opt.PayloadSerializerOptions.Converters
                .Add(new JsonStringEnumConverter()));

        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(c =>
            {
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme.
                                    Enter 'Bearer' [space] and then your token in the text input below.
                                    Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });

        return services;
    }

    public static IServiceCollection AddWorkers(this IServiceCollection services)
    {
        services
            .AddHostedService<SubscribeHandlersBackgroundService>()
            .AddHostedService<RedisIndexCreationService>()
            .AddHostedService<AgentBackgroundService>();

        return services;
    }
}
