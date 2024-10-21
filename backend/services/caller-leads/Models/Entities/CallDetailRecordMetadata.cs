namespace Plat4Me.DialLeadCaller.Application.Models.Entities;

public class CallDetailRecordMetadata
{
    public List<CallDetailRecordUser> Users { get; set; } = new();
    public CallDetailRecordDroppedInfo? DroppedInfo { get; set; }
    public string? ReasonDetails { get; set; }
    public string? AgentComment { get; set; }
    public string? ManagerComment { get; set; }
}

public record CallDetailRecordUser(
    long UserId,
    string? UserName);

public record CallDetailRecordDroppedInfo(
        long AgentId,
        long ManagerId,
        DateTimeOffset DroppedAt,
        string? Comment
    );
