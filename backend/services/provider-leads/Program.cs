using Plat4Me.Core.HealthCheck;
using Plat4Me.DialLeadProvider;
using Plat4Me.DialLeadProvider.Application.Configurations;
using Plat4Me.DialLeadProvider.Persistent.Configurations;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
//Services
builder.Configuration
    .AddEnvironmentVariables();

builder.Services
    .AddControllers();

builder.Host
    .UseSerilog((ctx, config)
        =>
    {
        config.ReadFrom.Configuration(ctx.Configuration);
    });

builder.Services
    .AddCoreHealthCheck()
    .ForwardToPrometheus();

builder.Services
    .AddApplication(builder.Configuration)
    .AddPersistent(builder.Configuration);

//Application
var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.MapMetrics();
app.UseRouting();
app.MapControllers();

app.Run();