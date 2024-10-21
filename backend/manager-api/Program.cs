using Microsoft.AspNetCore.Identity;
using Plat4Me.Authentication.Configurations;
using Plat4Me.Core.HealthCheck;
using Plat4Me.DialClientApi.Application.Configurations;
using Plat4Me.DialClientApi.Configurations;
using Plat4Me.DialClientApi.Middlewares;
using Plat4Me.DialClientApi.Persistent.Configurations;
using Plat4Me.DialClientApi.Persistent.Seed.Authentication;
using Plat4Me.DialClientApi.SignalR;
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
    .AddCustomControllers()
    .AddCustomSignalR(builder.Configuration)
    .AddSwagger();

builder.Services.AddPersistent(builder.Configuration);

builder.Services.AddApplication(builder.Configuration);

builder.Services.AddWorkers(builder.Configuration);

//Application
var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseCors("CorsPolicy");

app.MapHub<TrackingHub>("/tracking");

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