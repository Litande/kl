namespace Plat4Me.DialClientApi.Application.Models.Responses.Dashboard;

public record AgentsWorkMode(
    int TotalAmount,
    int InTheCall,
    int WaitingForTheCall,
    int FillingFeedback,
    int InBreak
);