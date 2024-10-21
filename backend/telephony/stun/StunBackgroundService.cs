using SIPSorcery.Net;
using System.Net;
using Microsoft.Extensions.Options;
using Plat4Me.DialStun.App;

namespace Plat4Me.DialStun.Workers;

public class StunBackgroundService : BackgroundService
{
    private readonly ILogger<StunBackgroundService> _logger;
    private readonly StunOptions _stunOptions;

    public StunBackgroundService(
        IOptions<StunOptions> stunOptions,
        ILogger<StunBackgroundService> logger
    )
    {
        _logger = logger;
        _stunOptions = stunOptions.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        IPEndPoint primaryEndPoint = new IPEndPoint(IPAddress.Any, 3478);
        IPEndPoint secondaryEndPoint = new IPEndPoint(IPAddress.Any, 3479);

        // Create the two STUN listeners and wire up the STUN server.
        STUNListener primarySTUNListener = new STUNListener(primaryEndPoint);
        STUNListener secondarySTUNListener = new STUNListener(secondaryEndPoint);
        STUNServer stunServer = new STUNServer(primaryEndPoint, primarySTUNListener.Send, secondaryEndPoint, secondarySTUNListener.Send);
        primarySTUNListener.MessageReceived += stunServer.STUNPrimaryReceived;
        secondarySTUNListener.MessageReceived += stunServer.STUNSecondaryReceived;

        AddVerboseLogs(stunServer);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(200);
        }

        primarySTUNListener.Close();
        secondarySTUNListener.Close();
        stunServer.Stop();
    }

    private void AddVerboseLogs(STUNServer stunServer)
    {
        if (_logger.IsEnabled(LogLevel.Trace))
        {
            stunServer.STUNPrimaryRequestInTraceEvent += (localEndPoint, fromEndPoint, stunMessage) =>
            {
                _logger.LogTrace($"pri recv {localEndPoint}<-{fromEndPoint}: {stunMessage.ToString()}");
            };

            stunServer.STUNSecondaryRequestInTraceEvent += (localEndPoint, fromEndPoint, stunMessage) =>
            {
                _logger.LogTrace($"sec recv {localEndPoint}<-{fromEndPoint}: {stunMessage.ToString()}");
            };

            stunServer.STUNPrimaryResponseOutTraceEvent += (localEndPoint, fromEndPoint, stunMessage) =>
            {
                _logger.LogTrace($"pri send {localEndPoint}->{fromEndPoint}: {stunMessage.ToString()}");
            };

            stunServer.STUNSecondaryResponseOutTraceEvent += (localEndPoint, fromEndPoint, stunMessage) =>
            {
                _logger.LogTrace($"sec send {localEndPoint}->{fromEndPoint}: {stunMessage.ToString()}");
            };
        }
    }
}
