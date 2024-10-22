using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using KL.SIP.Bridge.Application.Configurations;
using KL.SIP.Bridge.Application.Connections;
using KL.SIP.Bridge.Application.Models;
using KL.SIP.Bridge.Application.Services;
using KL.SIP.Bridge.Application.Session;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.X509;
using SIPSorcery.Net;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace KL.SIP.Bridge.Application.Workers;

public class RTCBackgroundService : BackgroundService
{

    private const string AgentRole = "Agent";
    private const string ManagerRole = "Manager";
    private const string TokenParam = "token";
    private readonly IOptionsMonitor<RTCOptions> _rtcOptions;
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<RTCBackgroundService> _logger;
    private WebSocketServer? _webSocketServer;
    private RTCConfiguration _defaultRtcConfiguration;
    private readonly ICallService _callService;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
    };

    public RTCBackgroundService(
        ILogger<RTCBackgroundService> logger,
        IOptionsMonitor<RTCOptions> RTCOptions,
        IOptions<JwtOptions> jwtOptions,
        ICallService callService
        )
    {
        _rtcOptions = RTCOptions;
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
        _callService = callService;
        X509CertificateParser cp = new Org.BouncyCastle.X509.X509CertificateParser();
        _defaultRtcConfiguration = new RTCConfiguration
        {
            iceServers = new List<RTCIceServer>(),
            certificates2 = new List<RTCCertificate2>()
            {
                new RTCCertificate2()
                {
                    Certificate = cp.ReadCertificate(File.ReadAllBytes(_rtcOptions.CurrentValue.Certificate)),
                    PrivateKey = DtlsUtils.LoadPrivateKeyResource(_rtcOptions.CurrentValue.PrivateKey)
                }
            }
        };

        foreach (var iceServer in _rtcOptions.CurrentValue.IceServers)
        {
            _defaultRtcConfiguration.iceServers.Add(new RTCIceServer
            {
                username = iceServer.Username,
                credential = iceServer.Password,
                urls = iceServer.Urls
            }
            );
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _webSocketServer = new WebSocketServer(IPAddress.Any, _rtcOptions.CurrentValue.ListenPort, false)
        {
            AllowForwardedRequest = true
        };
        //_webSocketServer.SslConfiguration = {};
        _webSocketServer.AddWebSocketService<RTCPeer>(_rtcOptions.CurrentValue.WebSocketPath, (peer) =>
        {
            peer.CreatePeerConnection = async () => await CreatePeerConnection(peer);
        });

        _webSocketServer.Start();
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(200), stoppingToken);
        }
        _webSocketServer.Stop();
    }

    private async Task<RTCPeerConnection> CreatePeerConnection(RTCPeer peer)
    {
        _logger.LogDebug("Incoming rtc connection ID={peerId}", peer.ID);

        RTCPeerConnection connection;
        peer.Logger = _logger;
        var claims = await ProcessRequest(peer);
        if (claims is null)
        {
            connection = new RTCPeerConnection();
            connection.Dispose();
        }
        else
        {
            var rtcConf = _defaultRtcConfiguration;
            var iceServers = peer.Session!.CallData.IceServers;
            if (!_rtcOptions.CurrentValue.UseIceServers)
                rtcConf = new RTCConfiguration()
                {
                    certificates2 = _defaultRtcConfiguration.certificates2.ToList()
                };
            else if (iceServers != null && iceServers.Any())
            {
                _logger.LogDebug("Use iceservers {iceServers}", String.Join(";", iceServers));
                rtcConf = new RTCConfiguration
                {
                    iceServers = new List<RTCIceServer>(),
                    certificates2 = _defaultRtcConfiguration.certificates2.ToList()
                };

                foreach (var iceServer in iceServers)
                {
                    rtcConf.iceServers.Add(new RTCIceServer
                    {
                        urls = iceServer
                    });
                }
            }
            else
                _logger.LogDebug("Use default iceservers");
            var rtcConnection = new RTCConnection(_rtcOptions.CurrentValue, rtcConf, _logger);
            connection = rtcConnection.PeerConnection;
            connection.OnClosed += () =>
            {
                peer.CloseWebSocket();
            };

            if (!await PrepareRTCConnection(peer, rtcConnection, claims))
                connection.Dispose();
        }
        return connection;
    }

    private async Task<IDictionary<string, object>?> ProcessRequest(RTCPeer peer)
    {
        try
        {
            var session = _callService.GetSession(peer.Context.QueryString["session"] ?? "");
            if (session is null)
                return null;

            session.OnClosed += (id) =>
            {
                _logger.LogDebug("Session onClosed");
                peer.CloseWebSocket();
            };
            peer.Init(session, _rtcOptions.CurrentValue.WebSocketPingPeriod);
            return await ReadToken(peer.Context.QueryString[TokenParam]);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while setting session's agent");
        }
        return null;
    }

    private async Task<bool> PrepareRTCConnection(RTCPeer peer, RTCConnection connection, IDictionary<string, object> userClaims)
    {
        long userId = 0;
        if (!userClaims.TryGetValue(ClaimTypes.NameIdentifier, out var userIdStr)
            || !long.TryParse(userIdStr as string, out userId))
            return false;

        if (!userClaims.TryGetValue(ClaimTypes.Role, out var userRoleObj))
            return false;

        peer.UserRole = userRoleObj as string;
        if (peer.UserRole == AgentRole)
        {
            peer.OnWebSocketMessage = (data) =>
            {
                try
                {
                    var agentCmd = JsonSerializer.Deserialize<AgentCommand>(data, _jsonOptions);
                    if (agentCmd is not null && agentCmd.Command is not null)
                    {
                        _logger.LogDebug("AgentCmd received {}", JsonSerializer.Serialize(agentCmd));
                        peer.Session!.ProcessCommand(agentCmd);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "HangupRequest deserialization failed");
                }

                return false;
            };
               
            return peer.Session is not null ? await peer.Session!.SetAgentConnection(connection, userId) : false;
        }
        else if (peer.UserRole == ManagerRole)
        {
            peer.OnWebSocketMessage = (data) =>
            {
                try
                {
                    var managerCmd = JsonSerializer.Deserialize<ManagerCommand>(data, _jsonOptions);
                    if (managerCmd is not null && managerCmd.Command is not null)
                    {
                        _logger.LogDebug("AgentCmd received {}", JsonSerializer.Serialize(managerCmd));
                        peer.Session!.ProcessCommand(managerCmd);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "ManagerModeRequest deserialization failed");
                }

                return false;
            };
            if (peer.Session is not null)
               await peer.Session.AddManagerConnection(connection, userId);
            return true;
        }
        return false;
    }

    private async Task<IDictionary<string, object>?> ReadToken(string? token)
    {
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Authorization token missing");
            return null;
        }
        var handler = new JwtSecurityTokenHandler();
        var validations = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.Key)),
            LifetimeValidator = (before, expires, token, param) => expires > DateTime.UtcNow,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
        };
        var result = await handler.ValidateTokenAsync(token, validations);
        if (result.IsValid)
        {
            return result.Claims;
        }
        else
        {
            _logger.LogWarning("Token is not valid");
            return null;
        }
    }


    private class RTCPeer : WebRTCWebSocketPeer
    {
        public Func<string, bool>? OnWebSocketMessage;
        public ILogger Logger = null!;

        public string? UserRole { get; set; }
        public ICallSession? Session { get; protected set; }

        private readonly static byte[] EmptyBytes = new byte[0];

        private Timer? _pingTimer = null;

        public void Init(ICallSession session, int pingPeriod)
        {
            Session = session;
            _pingTimer = new Timer(Ping, null, TimeSpan.FromSeconds(pingPeriod), TimeSpan.FromSeconds(pingPeriod));
        }

        public void Ping(object? data)
        {
            Send(EmptyBytes);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            _pingTimer?.Dispose();
            _pingTimer = null;
            if (Session is not null)
                Logger.LogDebug("WS closed {code} {reason} {sessId} {agentId} {leadId}", e.Code, e.Reason, Session.Id, Session.CallData.AgentId, Session.CallData.LeadId);
            else
                Logger.LogDebug("WS closed {code} {reason}", e.Code, e.Reason);
            if (UserRole == ManagerRole)
                RTCPeerConnection.close();
            else
                Session?.OnAgentConnectionLost();
        }

        public void CloseWebSocket()
        {
            _pingTimer?.Dispose();
            _pingTimer = null;
            Session = null;
            Logger.LogDebug("CloseWebSocket method");
            Close();
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            if (OnWebSocketMessage is not null && OnWebSocketMessage(e.Data)) return;
            base.OnMessage(e);
        }
    }
}
