namespace Plat4Me.Dial.Statistic.Api.Application.Models.Responses;

public record AgentsWorkMode(
    int TotalAmount,
    int InTheCall,
    int WaitingForTheCall,
    int FillingFeedback,
    int InBreak
);