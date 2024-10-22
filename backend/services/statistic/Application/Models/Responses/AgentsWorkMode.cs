namespace KL.Statistics.Application.Models.Responses;

public record AgentsWorkMode(
    int TotalAmount,
    int InTheCall,
    int WaitingForTheCall,
    int FillingFeedback,
    int InBreak
);