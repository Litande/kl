namespace Plat4Me.DialLeadCaller.Application.Models;

public record ValueChanges<T>(string Name, T From, T To);
