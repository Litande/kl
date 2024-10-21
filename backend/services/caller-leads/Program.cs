using Plat4Me.Core.HealthCheck;
using Plat4Me.DialLeadCaller;
using Plat4Me.DialLeadCaller.Application.App;
using Plat4Me.DialLeadCaller.Infrastructure.App;
using Plat4Me.DialLeadCaller.Seed;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//Services
builder.Configuration
    .AddEnvironmentVariables();

builder.Services.AddSwaggerGen();

builder.Services.AddCors(options => options
    .AddPolicy("CorsPolicy", p => p
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()));

builder.Services
    .AddControllers();

builder.Host
    .UseSerilog((ctx, config) =>
    {
        config.ReadFrom.Configuration(ctx.Configuration);
    });

builder.Services
    .AddCoreHealthCheck()
    .ForwardToPrometheus();

builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

builder.Services.InitializeDbData();

//Application
var app = builder.Build();

app.UseCors("CorsPolicy");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.MapMetrics();
app.UseRouting();

app.MapControllers();

app.Run();