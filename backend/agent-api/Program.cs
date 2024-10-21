using Microsoft.AspNetCore.Identity;
using Plat4Me.Authentication.Configurations;
using Plat4Me.Core.HealthCheck;
using Plat4Me.DialAgentApi.Application.Configurations;
using Plat4Me.DialAgentApi.Configurations;
using Plat4Me.DialAgentApi.Middlewares;
using Plat4Me.DialAgentApi.Persistent.Configurations;
using Plat4Me.DialAgentApi.Persistent.Seed.Authentication;
using Plat4Me.DialAgentApi.SignalR;
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
    .AddPlatAuthentication<IdentityUser<long>, IdentityRole<long>, long, JwtTokenGeneratorDial>(builder.Configuration, "dial");

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
