// using Prometheus;
// using Plat4Me.Core.HealthCheck;
using Serilog;

using Plat4Me.DialSipBridge.Application.Configurations;
using Plat4Me.DialSipBridge.Application.Services;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

using Plat4Me.DialSipBridge.Application.Models.Messages;
using Plat4Me.DialSipBridge.Application.Enums;
using Plat4me.Core.Nats;
using System.Web;
using Plat4Me.DialSipBridge.Application.Models;

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

builder.Services
    .AddControllers()
    .AddJsonOptions(opt => opt.JsonSerializerOptions.Converters
        .Add(new JsonStringEnumConverter()));

builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(
            builder => builder
                .SetIsOriginAllowed(_ => true)
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .Build());
        options.AddPolicy("CorsPolicy",
                policy =>
                policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
        );
    });

builder.Services
    .AddControllers();

builder.Services
    .AddApplication(builder.Configuration);

//Application
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();

//app.MapCoreHealthCheck();

// app.MapGet("/leadcall", async (string phoneNum, long agentId) =>
// {
//     ICallService srvs = app.Services.GetService<ICallService>()!;
//     return (await srvs.CreateSession(Guid.NewGuid().ToString(), new InitCallData(1, phoneNum, agentId, false))).ToString();
// });

app.MapGet("/sessionexists/{sessionId}", (string sessionId, CancellationToken ct) =>
{
    ICallService? callService = app.Services.GetService<ICallService>();
    if (callService is null)
        return Results.Problem();
    var session = callService.GetSession(sessionId);
    if (session is null)
        return Results.NotFound();
    return Results.Ok();
});

app.MapGet("/ping", () =>
    {
        IOptions<GeneralOptions>? opts = app.Services.GetService<IOptions<GeneralOptions>>();
        return Results.Content(opts?.Value.InstanceId ?? "", "text/plain");
    });

int sessionId = 0;


app.MapGet("/calltolead", async (long? leadid, string phoneNum, long agentId, bool? manual, bool? isTest) =>
    {
        var opts = app.Services.GetService<IOptions<NatsSubjects>>()!.Value;
        var sessOpts = app.Services.GetService<IOptions<CallSessionOptions>>()!.Value;
        var publisher = app.Services.GetService<INatsPublisher>()!;

        string sid = sessionId.ToString();
        ++sessionId;
        if (manual.HasValue && manual.Value)
        {
            await publisher.PublishAsync<CallToLeadMessage>(opts.TryCallToLead,
                new CallToLeadMessage(1, "test-bridge", sid, CallType.Manual, null, null, false, phoneNum, agentId, 60, 2400, null, isTest ?? false, "",
                    new SipProviderInfo(
                         1, "test", "", "", SipProviderStatus.Enable
                    )));
        }
        else
        {
            await publisher.PublishAsync<CallToLeadMessage>(opts.TryCallToLead,
                new CallToLeadMessage(1, "test-bridge", sid, CallType.Predictive, 1, leadid, false, phoneNum, agentId, 60, 2400, null, isTest ?? false, "",
                  new SipProviderInfo(
                         1, "test", "", "", SipProviderStatus.Enable
                    )));
        }

        return PrepareRtcUrl(sessOpts.AccessUrl, sid);

        string PrepareRtcUrl(string accessUrl, string id)
        {
            var url = new UriBuilder(accessUrl);
            var httpValueCollection = HttpUtility.ParseQueryString(url.Query);
            httpValueCollection.Remove("session");
            httpValueCollection.Add("session", id);
            url.Query = httpValueCollection.ToString();
            return url.ToString();
        }
    }
    );

app.MapGet("/hangupcall/{sessionId}", async (string sessionId) =>
    {
        var publisher = app.Services.GetService<INatsPublisher>()!;
        await publisher.PublishAsync<HangupCallMessage>("HangupCall",
            new HangupCallMessage(sessionId, ""));
    });

app.Run();
