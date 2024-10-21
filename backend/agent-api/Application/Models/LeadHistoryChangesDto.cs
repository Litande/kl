namespace Plat4Me.DialAgentApi.Application.Models;

public class LeadHistoryChangesDto<T>
{
    public List<ValueChanges<T>> Properties { get; set; } = new();
    public string Version { get; set; } = "1.0";
}
