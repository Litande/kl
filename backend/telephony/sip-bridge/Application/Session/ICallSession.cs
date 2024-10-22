using KL.SIP.Bridge.Application.Connections;
using KL.SIP.Bridge.Application.Enums;
using KL.SIP.Bridge.Application.Models;

namespace KL.SIP.Bridge.Application.Session;

public class CloseCommand
{
    public CallFinishReasons ReasonCode { get; set; }
    public string? ReasonDetails { get; set; }
    public string? AgentComment { get; set; }
    public string? ManagerComment { get; set; }
    public CloseCommand(CallFinishReasons reasonCode)
    {
        ReasonCode = reasonCode;
    }
}
public interface ICallSession
{
    string Id { get; }
    InitCallData CallData { get; }
    Task<bool> SetAgentConnection(IConnection connection, long agentId);
    void OnAgentConnectionLost();
    Task AddManagerConnection(IConnection connection, long userId);
    Task ReplaceAgent(long? agentId);
    Task Close(CloseCommand cmd);
    Task Start(InitCallData initCallData);
    Task ProcessCommand(AgentCommand agentCommand);
    Task ProcessCommand(ManagerCommand managerCommand);

    event Action<ICallSession>? OnClosed;
}
