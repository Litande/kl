namespace Plat4Me.DialLeadProvider.Application.Models;

public record ValueChanges<T>(string Name, T From, T To);