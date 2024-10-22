// using Prometheus;
// using Plat4Me.Core.HealthCheck;

using KL.Call.Mixer.App;
using Serilog;

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
