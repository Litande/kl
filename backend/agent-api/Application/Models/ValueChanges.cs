namespace KL.Agent.API.Application.Models;

public record ValueChanges<T>(string Name, T From, T To); 
