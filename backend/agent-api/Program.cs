using KL.Agent.API.Application.Configurations;
using KL.Agent.API.Configurations;
using KL.Agent.API.Middlewares;
using KL.Agent.API.Persistent;
using KL.Agent.API.Persistent.Configurations;
using KL.Agent.API.Persistent.Entities;
using KL.Agent.API.Persistent.Seed.Authentication;
using KL.Agent.API.SignalR;
using KL.Auth.Configurations;
using Microsoft.AspNetCore.Identity;
using Plat4Me.Core.HealthCheck;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .UseSerilog((ctx, config) =>
    {
        config.ReadFrom.Configuration(ctx.Configuration);
    });

builder.Services
    .AddCors(options => options
        .AddPolicy("CorsPolicy", config =>
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

builder.Services
    .AddAuthentication<User, Role, long, KlDbContext, JwtTokenGeneratorDial>(
        builder.Configuration, "kl");

builder.Services
    .AddHttpContextAccessor()
    .AddCustomControllers()
    .AddCustomSignalR()
    .AddSwagger();

builder.Services.AddPersistent(builder.Configuration);

builder.Services.AddApplication(builder.Configuration);

builder.Services.AddWorkers();

//Application
var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseCors("CorsPolicy");

app.MapHub<AgentHub>("/agent");

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


// declare class Program for testing
public partial class Program
{
}