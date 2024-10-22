using System.Text.Json.Serialization;
using KL.Auth.Configurations;
using KL.Nats;
using KL.Statistics.Application.SignalR;
using KL.Statistics.Authentication;
using KL.Statistics.Configurations;
using KL.Statistics.DAL.Configurations;
using KL.Statistics.Middlewares;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Plat4Me.Core.HealthCheck;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
//Services
builder.Host
    .UseSerilog((ctx, config) =>
    {
        config.ReadFrom.Configuration(ctx.Configuration);
    });

builder.Services.AddCors(options => options.AddPolicy("CorsPolicy", config =>
{
    config
        .SetIsOriginAllowed(_ => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
}));

builder.Services
    .AddCoreHealthCheck()
    .ForwardToPrometheus();

builder.Configuration
    .AddEnvironmentVariables();

builder.Services
    .AddAuthentication<IdentityUser<long>, IdentityRole<long>, long, JwtTokenGeneratorDial>(builder.Configuration, "kl");

builder.Services
    .AddSignalR()
    .AddJsonProtocol(opt => opt.PayloadSerializerOptions.Converters
        .Add(new JsonStringEnumConverter()));

builder.Services
    .AddControllers()
    .AddJsonOptions(opt => opt.JsonSerializerOptions.Converters
        .Add(new JsonStringEnumConverter()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
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

builder.Services.AddHandlers();
builder.Services.AddServices();

builder.Services.AddNatsCore(builder.Configuration);

builder.Services.AddServiceOptions(builder.Configuration);
builder.Services.AddRedisConfiguration(builder.Configuration);
builder.Services.AddWorkers();

builder.Services.AddScoped<IHubSender, HubSender>();
builder.Services
    .AddRepositories(builder.Configuration);

builder.Services.AddOptions<SignalROptions>()
    .Bind(builder.Configuration.GetSection("CLIENTS:SignalRChanel"));

builder.Services
    .AddOptions<SubSubjects>()
    .Bind(builder.Configuration.GetSection(nameof(SubSubjects)));

//Application
var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseCors("CorsPolicy");

app.MapHub<TrackingHub>("/statistics");

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapMetrics();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

namespace KL.Statistics
{
    partial class Program
    {
    }
}