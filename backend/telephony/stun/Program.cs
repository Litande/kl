// using Prometheus;
// using Plat4Me.Core.HealthCheck;
using Serilog;
using Plat4Me.DialStun.App;

var builder = WebApplication.CreateBuilder(args);
//Services
builder.Configuration
    .AddEnvironmentVariables();

builder.Host
    .UseSerilog((ctx, config)
        =>
    { config.ReadFrom.Configuration(ctx.Configuration); });

// builder.Services
//     .AddCoreHealthCheck()
//     .ForwardToPrometheus();

builder.Services.AddApplication(builder.Configuration);

//Application
var app = builder.Build();

app.Run();
