namespace Plat4Me.DialAgentApi.Application.Models;

public record ValueChanges<T>(string Name, T From, T To); 
