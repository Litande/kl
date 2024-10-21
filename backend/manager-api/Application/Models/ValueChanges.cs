namespace Plat4Me.DialClientApi.Application.Models;

public record ValueChanges<T>(string Name, T From, T To);
