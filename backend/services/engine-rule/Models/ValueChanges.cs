namespace Plat4Me.DialRuleEngine.Application.Models;

public record ValueChanges<T>(string Name, T From, T To); 
