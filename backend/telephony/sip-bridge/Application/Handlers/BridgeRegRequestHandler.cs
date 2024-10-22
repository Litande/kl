using System.Net;
using System.Net.Sockets;
using KL.Nats;
using KL.SIP.Bridge.Application.Configurations;
using KL.SIP.Bridge.Application.Models.Messages;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Options;

namespace KL.SIP.Bridge.Application.Handlers;

public class BridgeRegRequestHandler : IBridgeRegRequestHandler
{
    private readonly ILogger<BridgeRegRequestHandler> _logger;
    private readonly IOptions<NatsSubjects> _natsSubjects;
    private readonly IOptions<GeneralOptions> _generalOptions;
    private readonly INatsPublisher _natsPublisher;
    private readonly int _port = 80;

    public BridgeRegRequestHandler(
        IOptions<GeneralOptions> generalOptions,
        IOptions<NatsSubjects> natsSubjects,
        INatsPublisher natsPublisher,
        ILogger<BridgeRegRequestHandler> logger,
        IServer server
        )
    {
        _logger = logger;
        _natsSubjects = natsSubjects;
        _natsPublisher = natsPublisher;
        _generalOptions = generalOptions;
        var addresses = server.Features.Get<IServerAddressesFeature>()!.Addresses;
        var httpAddress = addresses.Where(x => x.StartsWith("http://")).FirstOrDefault();
        if (httpAddress is not null)
        {
            _port = (new Uri(httpAddress)).Port;
        }
    }

    public async Task Process(BridgeRegRequestMessage message, CancellationToken ct = default)
    {
        var instanceId = _generalOptions.Value.InstanceId;
        var strHostName = Dns.GetHostName();
        IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
        IPAddress[] addresses = ipEntry.AddressList;
        string? target = null;
        foreach (var ip in addresses)
        {
            if (IPAddress.IsLoopback(ip)) continue;
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                target = ip.ToString();
                break;
            }
        }
        if (string.IsNullOrWhiteSpace(target))
        {
            _logger.LogCritical("Can not get host ip addr");
            return;
        }
        var addr = $"{target}:{_port}";
        _logger.LogInformation("Public BridgeRun {instanceId} {addr}", instanceId, addr);
        await _natsPublisher.PublishAsync(_natsSubjects.Value.BridgeRun,
            new BridgeRunMessage(
                instanceId,
                addr
        ));
    }
}
